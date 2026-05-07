using System;
using System.IO;
using System.Linq;

namespace SmartResumeAnalyzer.Utils;

public static class AnalysisConstants
{
    // Score thresholds
    public const double MinConfidenceScore = 0.65;
    public const double SkillMatchThreshold = 0.7;
    public const double AtsCompatibilityThreshold = 60.0;

    // Content limits
    public const int MaxResumeLength = 10000;
    public const int MinResumeLength = 100;
    public const int RecommendedResumeLength = 3000;

    // Scoring weights
    public const double SkillsWeight = 0.25;
    public const double ExperienceWeight = 0.35;
    public const double EducationWeight = 0.20;
    public const double CertificationWeight = 0.15;
    public const double FormatWeight = 0.05;

    // Section scores
    public const double PointsPerSkill = 10.0;
    public const double PointsPerJobExperience = 25.0;
    public const double PointsPerEducation = 50.0;
    public const double PointsPerCertification = 25.0;

    // Common degree keywords
    public static readonly string[] DegreeKeywords = new[]
    {
        "bachelor", "bachelor's", "bs", "ba", "b.s.", "b.a.",
        "master", "master's", "ms", "ma", "m.s.", "m.a.",
        "doctorate", "phd", "ph.d.",
        "diploma", "associate", "a.s.", "a.a.",
        "certification program", "bootcamp"
    };

    // Common job title patterns
    public static readonly string[] JobTitleKeywords = new[]
    {
        "developer", "engineer", "architect", "analyst",
        "manager", "lead", "senior", "junior",
        "intern", "consultant", "specialist",
        "administrator", "coordinator", "associate"
    };

    // Experience indicators
    public static readonly string[] ExperienceKeywords = new[]
    {
        "experience", "worked", "managed", "developed", "designed",
        "implemented", "created", "built", "established", "led"
    };

    // ATS-unfriendly elements
    public static readonly string[] AtsUnfriendlyElements = new[]
    {
        "image", "table of contents", "graphics", "charts",
        "colored text", "unusual fonts", "headers", "footers"
    };
}

public static class ValidationHelper
{
    public static bool IsValidResumeLength(string content)
    {
        return content?.Length >= AnalysisConstants.MinResumeLength &&
               content?.Length <= AnalysisConstants.MaxResumeLength;
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidFileFormat(string fileName, string[] supportedFormats)
    {
        var extension = Path.GetExtension(fileName).ToLower().TrimStart('.');
        return supportedFormats.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    public static bool HasRequiredSections(string content)
    {
        var sections = new[] { "education", "experience", "skills" };
        return sections.All(s => 
            content.Contains(s, StringComparison.OrdinalIgnoreCase));
    }
}
