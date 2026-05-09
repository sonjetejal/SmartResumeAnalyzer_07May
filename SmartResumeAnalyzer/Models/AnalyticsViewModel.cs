using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartResumeAnalyzer.Models;

public class AnalyticsViewModel
{
    public int TotalResumes { get; set; }
    public int AnalyzedResumes { get; set; }
    public double AverageScore { get; set; }
    public double HighestScore { get; set; }
    public double LowestScore { get; set; }

    // Score distribution for histogram (0-20, 21-40, 41-60, 61-80, 81-100)
    public List<ScoreDistributionItem> ScoreDistribution { get; set; } = new();

    // Category scores averages
    public List<CategoryScoreItem> CategoryScores { get; set; } = new();

    // Top skills across all resumes
    public List<SkillFrequencyItem> TopSkills { get; set; } = new();

    // Monthly upload trend
    public List<MonthlyTrendItem> MonthlyTrend { get; set; } = new();

    // Education distribution
    public Dictionary<string, int> EducationDistribution { get; set; } = new();

    // Experience level distribution
    public List<ExperienceLevelItem> ExperienceLevels { get; set; } = new();
}

public class ScoreDistributionItem
{
    public string Range { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class CategoryScoreItem
{
    public string Category { get; set; } = string.Empty;
    public double AverageScore { get; set; }
}

public class SkillFrequencyItem
{
    public string SkillName { get; set; } = string.Empty;
    public int Frequency { get; set; }
}

public class MonthlyTrendItem
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
    public double AverageScore { get; set; }
}

public class ExperienceLevelItem
{
    public string Level { get; set; } = string.Empty;
    public int Count { get; set; }
}