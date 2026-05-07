using System;
using System.Collections.Generic;

namespace SmartResumeAnalyzer.Models;

public class Resume
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileFormat { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; } = DateTime.Now;
    public ResumeAnalysisResult? Analysis { get; set; }
}

public class ResumeAnalysisResult
{
    public double OverallScore { get; set; }
    public List<string> ExtractedSkills { get; set; } = new();
    public List<SkillMatch> SkillMatches { get; set; } = new();
    public List<string> Certifications { get; set; } = new();
    public List<string> Education { get; set; } = new();
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public string SummaryAnalysis { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
    public Dictionary<string, double> CategoryScores { get; set; } = new();
    public DateTime AnalyzedDate { get; set; } = DateTime.Now;
}

public class SkillMatch
{
    public string SkillName { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string Category { get; set; } = string.Empty; // Technical, Soft, etc.
    public int Frequency { get; set; }
}

public class WorkExperience
{
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> SkillsUsed { get; set; } = new();
}

public class ResumeAnalysisRequest
{
    public string FileName { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileFormat { get; set; } = string.Empty;
    public string? JobDescription { get; set; }
}

public class AnalysisMetrics
{
    public double SkillCoverage { get; set; }
    public double ExperienceScore { get; set; }
    public double EducationScore { get; set; }
    public double CertificationScore { get; set; }
    public double FormatScore { get; set; }
    public double ATS_Compatibility { get; set; }
}

public class JobRequirement
{
    public string RequiredSkill { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int YearsRequired { get; set; }
}
