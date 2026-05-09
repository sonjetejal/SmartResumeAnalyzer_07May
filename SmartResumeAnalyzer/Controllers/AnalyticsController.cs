using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartResumeAnalyzer.Models;
using SmartResumeAnalyzer.Services;

namespace SmartResumeAnalyzer.Controllers;

public class AnalyticsController : Controller
{
    private readonly IResumeStorageService _storageService;

    public AnalyticsController(IResumeStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<IActionResult> Index()
    {
        var allResumes = await _storageService.GetAllResumesAsync();
        var model = new AnalyticsViewModel();

        if (allResumes == null || !allResumes.Any())
        {
            return View(model);
        }

        // Basic counts
        model.TotalResumes = allResumes.Count;
        var analyzedResumes = allResumes.Where(r => r.Analysis != null).ToList();
        model.AnalyzedResumes = analyzedResumes.Count;

        if (!analyzedResumes.Any())
        {
            return View(model);
        }

        // Score statistics
        var scores = analyzedResumes.Select(r => r.Analysis!.OverallScore).ToList();
        model.AverageScore = Math.Round(scores.Average(), 2);
        model.HighestScore = scores.Max();
        model.LowestScore = scores.Min();

        // Score distribution (0-20, 21-40, 41-60, 61-80, 81-100)
        model.ScoreDistribution = new List<ScoreDistributionItem>
        {
            new ScoreDistributionItem { Range = "0-20", Count = scores.Count(s => s >= 0 && s <= 20) },
            new ScoreDistributionItem { Range = "21-40", Count = scores.Count(s => s > 20 && s <= 40) },
            new ScoreDistributionItem { Range = "41-60", Count = scores.Count(s => s > 40 && s <= 60) },
            new ScoreDistributionItem { Range = "61-80", Count = scores.Count(s => s > 60 && s <= 80) },
            new ScoreDistributionItem { Range = "81-100", Count = scores.Count(s => s > 80 && s <= 100) }
        };

        // Category scores averages
        var categoryScoresDict = new Dictionary<string, List<double>>();
        foreach (var resume in analyzedResumes)
        {
            if (resume.Analysis?.CategoryScores != null)
            {
                foreach (var kv in resume.Analysis.CategoryScores)
                {
                    if (!categoryScoresDict.ContainsKey(kv.Key))
                        categoryScoresDict[kv.Key] = new List<double>();
                    categoryScoresDict[kv.Key].Add(kv.Value);
                }
            }
        }

        model.CategoryScores = categoryScoresDict
            .Select(kv => new CategoryScoreItem
            {
                Category = kv.Key,
                AverageScore = Math.Round(kv.Value.Average(), 2)
            })
            .OrderByDescending(c => c.AverageScore)
            .ToList();

        // Top skills frequency
        var skillCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        static string NormalizeSkill(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
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

        foreach (var r in analyzedResumes)
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

        model.TopSkills = skillCounts
            .OrderByDescending(kv => kv.Value)
            .Take(10)
            .Select(kv => new SkillFrequencyItem
            {
                SkillName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(kv.Key),
                Frequency = kv.Value
            })
            .ToList();

        // Monthly trend (last 6 months)
        var sixMonthsAgo = DateTime.Now.AddMonths(-6);
        var monthlyData = allResumes
            .Where(r => r.UploadedDate >= sixMonthsAgo)
            .GroupBy(r => new { Year = r.UploadedDate.Year, Month = r.UploadedDate.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var monthResumes = g.ToList();
                var monthAnalyzed = monthResumes.Where(r => r.Analysis != null).ToList();
                return new MonthlyTrendItem
                {
                    Month = $"{GetMonthName(g.Key.Month)} {g.Key.Year}",
                    Count = monthResumes.Count,
                    AverageScore = monthAnalyzed.Any() 
                        ? Math.Round(monthAnalyzed.Average(r => r.Analysis!.OverallScore), 2) 
                        : 0
                };
            })
            .ToList();

        // Fill in missing months
        var allMonths = new List<MonthlyTrendItem>();
        for (int i = 5; i >= 0; i--)
        {
            var targetMonth = DateTime.Now.AddMonths(-i);
            var monthName = $"{GetMonthName(targetMonth.Month)} {targetMonth.Year}";
            var existing = monthlyData.FirstOrDefault(m => m.Month == monthName);
            if (existing != null)
            {
                allMonths.Add(existing);
            }
            else
            {
                allMonths.Add(new MonthlyTrendItem
                {
                    Month = monthName,
                    Count = 0,
                    AverageScore = 0
                });
            }
        }
        model.MonthlyTrend = allMonths;

        // Education distribution
        var educationCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var resume in analyzedResumes)
        {
            if (resume.Analysis?.Education == null) continue;
            foreach (var edu in resume.Analysis.Education)
            {
                var normalized = CategorizeEducation(edu);
                if (!string.IsNullOrEmpty(normalized))
                {
                    if (!educationCounts.ContainsKey(normalized))
                        educationCounts[normalized] = 0;
                    educationCounts[normalized]++;
                }
            }
        }
        model.EducationDistribution = educationCounts;

        // Experience level distribution
        var expLevels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var resume in analyzedResumes)
        {
            if (resume.Analysis?.WorkExperiences == null) continue;
            var totalYears = 0;
            foreach (var exp in resume.Analysis.WorkExperiences)
            {
                var years = ExtractYears(exp.Duration);
                totalYears += years;
            }

            string level;
            if (totalYears == 0) level = "Fresher";
            else if (totalYears <= 2) level = "Junior (0-2 yrs)";
            else if (totalYears <= 5) level = "Mid-Level (3-5 yrs)";
            else if (totalYears <= 10) level = "Senior (6-10 yrs)";
            else level = "Expert (10+ yrs)";

            if (!expLevels.ContainsKey(level))
                expLevels[level] = 0;
            expLevels[level]++;
        }

        model.ExperienceLevels = expLevels
            .Select(kv => new ExperienceLevelItem { Level = kv.Key, Count = kv.Value })
            .OrderBy(e => e.Level)
            .ToList();

        return View(model);
    }

    private string GetMonthName(int month)
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
    }

    private string CategorizeEducation(string education)
    {
        if (string.IsNullOrWhiteSpace(education)) return string.Empty;
        var lower = education.ToLowerInvariant();

        if (lower.Contains("phd") || lower.Contains("doctorate")) return "PhD/Doctorate";
        if (lower.Contains("master") || lower.Contains("m.") || lower.Contains("mtech") || lower.Contains("msc") || lower.Contains("mba")) return "Master's Degree";
        if (lower.Contains("bachelor") || lower.Contains("b.") || lower.Contains("btech") || lower.Contains("bsc") || lower.Contains("bcom") || lower.Contains("ba ")) return "Bachelor's Degree";
        if (lower.Contains("diploma") || lower.Contains("certificate")) return "Diploma/Certificate";
        if (lower.Contains("12th") || lower.Contains("hsc") || lower.Contains("higher secondary")) return "High School";
        if (lower.Contains("10th") || lower.Contains("ssc") || lower.Contains("secondary")) return "Secondary School";

        return "Other";
    }

    private int ExtractYears(string duration)
    {
        if (string.IsNullOrWhiteSpace(duration)) return 0;

        // Try to extract years from patterns like "2 years", "2019-2021", "3+ years", etc.
        var lower = duration.ToLowerInvariant();
        
        // Pattern: "X years" or "X+ years"
        var match = System.Text.RegularExpressions.Regex.Match(lower, @"(\d+)\+?\s*years?");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var years))
            return years;

        // Pattern: "YYYY-YYYY" or "YYYY - YYYY"
        match = System.Text.RegularExpressions.Regex.Match(duration, @"(\d{4})\s*-\s*(\d{4}|\bpresent\b)");
        if (match.Success)
        {
            if (int.TryParse(match.Groups[1].Value, out var startYear))
            {
                if (match.Groups[2].Value.ToLower() == "present")
                {
                    return DateTime.Now.Year - startYear;
                }
                if (int.TryParse(match.Groups[2].Value, out var endYear))
                {
                    return endYear - startYear;
                }
            }
        }

        // Pattern: "X months"
        match = System.Text.RegularExpressions.Regex.Match(lower, @"(\d+)\s*months?");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var months))
            return months / 12;

        return 0;
    }
}