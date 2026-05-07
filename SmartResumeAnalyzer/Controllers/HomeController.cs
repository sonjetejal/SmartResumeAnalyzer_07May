using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartResumeAnalyzer.Services;

namespace SmartResumeAnalyzer.Controllers;

public class HomeController : Controller
{
    private readonly IResumeStorageService _storageService;

    public HomeController(IResumeStorageService storageService)
    {
        _storageService = storageService;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> About()
    {
        var all = await _storageService.GetAllResumesAsync();
        ViewBag.TotalResumes = all?.Count ?? 0;
        var last = all?.OrderByDescending(r => r.UploadedDate).FirstOrDefault();
        ViewBag.LastUploaded = last?.UploadedDate.ToString("f") ?? "—";
        ViewBag.LastFile = last?.FileName ?? "—";
        ViewBag.LastFileId = last?.Id ?? string.Empty;

        // Compute top extracted skills across all resumes (normalized, aggregated)
        var skillCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        static string NormalizeSkill(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            // remove diacritics and normalize casing
            var form = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in form)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            var cleaned = sb.ToString().Normalize(NormalizationForm.FormC);
            return cleaned.Trim().ToLowerInvariant();
        }

        if (all != null)
        {
            foreach (var r in all)
            {
                if (r.Analysis?.ExtractedSkills == null) continue;
                foreach (var s in r.Analysis.ExtractedSkills)
                {
                    var n = NormalizeSkill(s);
                    if (string.IsNullOrEmpty(n)) continue;
                    if (skillCounts.ContainsKey(n)) skillCounts[n]++;
                    else skillCounts[n] = 1;
                }
            }
        }

        // (Do not force any skill counts here; top skills will reflect actual extracted skills.)

        var topSkills = skillCounts.OrderByDescending(kv => kv.Value)
                                  .Take(8) // increase to show more
                                  .Select(kv => new { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(kv.Key), Count = kv.Value })
                                  .ToList();
        ViewBag.TopSkills = topSkills;
        // Recent files for quick download links
        var recentFilesAnon = all?.OrderByDescending(r => r.UploadedDate)
                              .Take(6)
                              .Select(r => new { r.Id, r.FileName, Uploaded = r.UploadedDate.ToString("g") })
                              .ToList();
        var recentFiles = recentFilesAnon?.Cast<object>().ToList() ?? new List<object>();
        ViewBag.RecentFiles = recentFiles;
        // Show storage directory where files are saved
        try
        {
            var storageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resumes");
            ViewBag.StorageDirectory = Directory.Exists(storageDir) ? storageDir : "Not created yet";
        }
        catch
        {
            ViewBag.StorageDirectory = "Unavailable";
        }
        // Additional unique metrics for About page
        var analyzedList = all?.Where(r => r.Analysis != null).ToList() ?? new List<Models.Resume>();
        ViewBag.AnalyzedCount = analyzedList.Count;
        ViewBag.AverageScore = analyzedList.Count > 0 ? analyzedList.Average(r => r.Analysis!.OverallScore) : 0.0;
        // prefer the most recently uploaded resume that has been analyzed
        var lastAnalyzed = analyzedList.OrderByDescending(r => r.UploadedDate).FirstOrDefault();
        ViewBag.LastAnalyzed = lastAnalyzed?.Analysis?.AnalyzedDate.ToString("f") ?? "—";

        // If there is a most recently analyzed resume, show its extracted skills as TopSkills
        if (lastAnalyzed?.Analysis?.ExtractedSkills != null && lastAnalyzed.Analysis.ExtractedSkills.Count > 0)
        {
            var lastSkills = lastAnalyzed.Analysis.ExtractedSkills
                .Select(s => NormalizeSkill(s))
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(8)
                .Select(s => new { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s), Count = 1 })
                .ToList();
            ViewBag.TopSkills = lastSkills;
        }

        // Recent downloads display removed
        return View();
    }

    public IActionResult Docs() => View();

    public IActionResult Privacy() => View();

    public IActionResult Error() => View();
}
