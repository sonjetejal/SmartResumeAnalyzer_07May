using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartResumeAnalyzer.Models;
using SmartResumeAnalyzer.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SmartResumeAnalyzer.Controllers;

public class ResumeController : Controller
{
    private readonly IResumeAnalysisService _analysisService;
    private readonly IResumeStorageService _storageService;
    private readonly ILogger<ResumeController> _logger;

    public ResumeController(
        IResumeAnalysisService analysisService,
        IResumeStorageService storageService,
        ILogger<ResumeController> logger)
    {
        _analysisService = analysisService;
        _storageService = storageService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Analyze() => View();

    [HttpPost]
    public async Task<IActionResult> Analyze(IFormFile resumeFile, string? jobDescription)
    {
        if (resumeFile == null || resumeFile.Length == 0)
        {
            ModelState.AddModelError("", "Please select a resume file.");
            return View();
        }

        try
        {
            using var ms = new MemoryStream();
            await resumeFile.CopyToAsync(ms);
            using var doc = WordprocessingDocument.Open(ms, false);

            var body = doc.MainDocumentPart?.Document?.Body;
            var content = body?.InnerText;
            //var content = System.Text.Encoding.UTF8.GetString(ms.ToArray());

            var resume = new Resume
            {
                FileName = resumeFile.FileName,
                Content = content,
                FileFormat = Path.GetExtension(resumeFile.FileName)
            };

            ResumeAnalysisResult analysis;
            if (!string.IsNullOrWhiteSpace(jobDescription))
                analysis = await _analysisService.AnalyzeResumeAgainstJobAsync(resume, jobDescription);
            else
                analysis = await _analysisService.AnalyzeResumeAsync(resume);

            resume.Analysis = analysis;
            await _storageService.SaveResumeAsync(resume);

            return RedirectToAction(nameof(Results), new { id = resume.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing resume");
            ModelState.AddModelError("", $"Error analyzing resume: {ex.Message}");
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Results(string id)
    {
        var resume = await _storageService.GetResumeAsync(id);
        if (resume?.Analysis == null)
            return NotFound();

        return View(resume);
    }

    [HttpGet]
    public async Task<IActionResult> Download(string id)
    {
        var resume = await _storageService.GetResumeAsync(id);
        if (resume == null)
            return NotFound();

        // Prepare bytes from stored text content. Original binary may not be preserved for all formats;
        // the app stores uploaded file content as UTF8 text earlier, so download will return that text.
        var bytes = System.Text.Encoding.UTF8.GetBytes(resume.Content ?? string.Empty);

        var ext = (resume.FileFormat ?? string.Empty).ToLowerInvariant();
        var contentType = ext switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".doc" => "application/msword",
            ".txt" => "text/plain",
            _ => "application/octet-stream",
        };

        // record download event
        try
        {
            await _storageService.RecordDownloadAsync(resume.Id);
        }
        catch
        {
            // ignore logging failures here
        }

        return File(bytes, contentType, resume.FileName ?? "resume");
    }

    [HttpGet]
    public IActionResult Search() => View();

    [HttpPost]
    public async Task<IActionResult> Search(string query)
    {
        var results = await _storageService.SearchResumesAsync(query ?? string.Empty);
        ViewBag.Query = query;
        return View(results);
    }

    [HttpGet]
    public async Task<IActionResult> ATSScore(string id)
    {
        var resume = await _storageService.GetResumeAsync(id);
        if (resume == null)
            return NotFound();

        var score = await _analysisService.CalculateAtsScoreAsync(resume);
        ViewBag.Score = score;
        return View(resume);
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var all = await _storageService.SearchResumesAsync(string.Empty);

        // get recent downloads and compute counts per resume id
        var downloads = await _storageService.GetRecentDownloadsAsync(200);
        var downloadCounts = downloads.GroupBy(d => d.Id).ToDictionary(g => g.Key, g => g.Count());
        ViewBag.DownloadCounts = downloadCounts;

        // prepare recent download details (filename + time)
        var recentDetails = new List<object>();
        foreach (var d in downloads.Take(10))
        {
            var r = await _storageService.GetResumeAsync(d.Id);
            recentDetails.Add(new { Id = d.Id, FileName = r?.FileName ?? d.Id, Downloaded = d.Downloaded });
        }
        ViewBag.RecentDownloads = recentDetails;

        return View(all);
    }
}
