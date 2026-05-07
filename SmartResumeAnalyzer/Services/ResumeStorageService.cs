using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SmartResumeAnalyzer.Models;

namespace SmartResumeAnalyzer.Services;

public interface IResumeStorageService
{
    Task<Resume> SaveResumeAsync(Resume resume);
    Task<Resume?> GetResumeAsync(string resumeId);
    Task<List<Resume>> GetAllResumesAsync();
    Task DeleteResumeAsync(string resumeId);
    Task<List<Resume>> SearchResumesAsync(string query);
    Task RecordDownloadAsync(string resumeId);
    Task<List<(string Id, DateTime Downloaded)>> GetRecentDownloadsAsync(int max);
}

public class ResumeStorageService : IResumeStorageService
{
    private readonly ILogger<ResumeStorageService> _logger;
    private readonly string _storageDirectory;
    private readonly Dictionary<string, Resume> _inMemoryStorage;

    public ResumeStorageService(ILogger<ResumeStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _storageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resumes");
        _inMemoryStorage = new Dictionary<string, Resume>();

        if (!Directory.Exists(_storageDirectory))
        {
            Directory.CreateDirectory(_storageDirectory);
        }

    }

    public async Task RecordDownloadAsync(string resumeId)
    {
        try
        {
            var logPath = Path.Combine(_storageDirectory, "downloads.log");
            var line = $"{resumeId}|{DateTime.UtcNow:o}{Environment.NewLine}";
            await File.AppendAllTextAsync(logPath, line);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording download for {ResumeId}", resumeId);
        }
    }

    public async Task<List<(string Id, DateTime Downloaded)>> GetRecentDownloadsAsync(int max)
    {
        try
        {
            var logPath = Path.Combine(_storageDirectory, "downloads.log");
            if (!File.Exists(logPath)) return new List<(string, DateTime)>();

            var lines = await File.ReadAllLinesAsync(logPath);
            var parsed = new List<(string Id, DateTime Downloaded)>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('|');
                if (parts.Length != 2) continue;
                if (DateTime.TryParse(parts[1], null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                {
                    parsed.Add((parts[0], dt));
                }
            }

            // return most recent first
            return parsed.OrderByDescending(p => p.Downloaded).Take(max).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading download log");
            return new List<(string, DateTime)>();
        }
    }

    public async Task<Resume> SaveResumeAsync(Resume resume)
    {
        try
        {
            _inMemoryStorage[resume.Id] = resume;
            
            // Also save to file system
            var filePath = Path.Combine(_storageDirectory, $"{resume.Id}.json");
            var json = System.Text.Json.JsonSerializer.Serialize(resume);
            await File.WriteAllTextAsync(filePath, json);

            _logger.LogInformation("Resume saved with ID: {ResumeId}", resume.Id);
            return resume;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving resume");
            throw;
        }
    }

    public async Task<Resume?> GetResumeAsync(string resumeId)
    {
        try
        {
            if (_inMemoryStorage.TryGetValue(resumeId, out var resume))
            {
                return resume;
            }

            var filePath = Path.Combine(_storageDirectory, $"{resumeId}.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Resume not found: {ResumeId}", resumeId);
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var loaded = System.Text.Json.JsonSerializer.Deserialize<Resume>(json);
            
            if (loaded != null)
            {
                _inMemoryStorage[resumeId] = loaded;
            }

            return loaded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving resume: {ResumeId}", resumeId);
            return null;
        }
    }

    public async Task<List<Resume>> GetAllResumesAsync()
    {
        try
        {
            var resumes = _inMemoryStorage.Values.ToList();
            
            var files = Directory.GetFiles(_storageDirectory, "*.json");
            foreach (var file in files)
            {
                var resumeId = Path.GetFileNameWithoutExtension(file);
                if (!_inMemoryStorage.ContainsKey(resumeId))
                {
                    var json = await File.ReadAllTextAsync(file);
                    var resume = System.Text.Json.JsonSerializer.Deserialize<Resume>(json);
                    if (resume != null)
                    {
                        resumes.Add(resume);
                        _inMemoryStorage[resumeId] = resume;
                    }
                }
            }

            return resumes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all resumes");
            return new List<Resume>();
        }
    }

    public async Task DeleteResumeAsync(string resumeId)
    {
        try
        {
            _inMemoryStorage.Remove(resumeId);

            var filePath = Path.Combine(_storageDirectory, $"{resumeId}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _logger.LogInformation("Resume deleted: {ResumeId}", resumeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting resume: {ResumeId}", resumeId);
            throw;
        }
    }

    public async Task<List<Resume>> SearchResumesAsync(string query)
    {
        try
        {
            var allResumes = await GetAllResumesAsync();
            
            return allResumes.Where(r => 
                r.Content.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.FileName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.Analysis?.ExtractedSkills.Any(s => s.Contains(query, StringComparison.OrdinalIgnoreCase)) == true
            ).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching resumes");
            return new List<Resume>();
        }
    }
}
