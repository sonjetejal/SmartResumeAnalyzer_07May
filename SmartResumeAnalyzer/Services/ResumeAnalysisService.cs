using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SmartResumeAnalyzer.Models;
using SmartResumeAnalyzer.Data;

namespace SmartResumeAnalyzer.Services;

public interface IResumeAnalysisService
{
    Task<ResumeAnalysisResult> AnalyzeResumeAsync(Resume resume);
    Task<ResumeAnalysisResult> AnalyzeResumeAgainstJobAsync(Resume resume, string jobDescription);
    Task<double> CalculateAtsScoreAsync(Resume resume);
    Task<List<string>> GenerateRecommendationsAsync(ResumeAnalysisResult analysis);
}

public class ResumeAnalysisService : IResumeAnalysisService
{
    private readonly IMLPredictionService _mlPredictionService;
    private readonly ITextExtractionService _textExtractionService;
    private readonly ILogger<ResumeAnalysisService> _logger;
    private readonly IConfiguration _configuration;

    public ResumeAnalysisService(
        IMLPredictionService mlPredictionService,
        ITextExtractionService textExtractionService,
        ILogger<ResumeAnalysisService> logger,
        IConfiguration configuration)
    {
        _mlPredictionService = mlPredictionService;
        _textExtractionService = textExtractionService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ResumeAnalysisResult> AnalyzeResumeAsync(Resume resume)
    {
        _logger.LogInformation("Starting analysis for resume: {ResumeId}", resume.Id);

        try
        {
            var analysis = new ResumeAnalysisResult();

            // Extract content if needed
            if (string.IsNullOrEmpty(resume.Content))
            {
                resume.Content = await _textExtractionService.ExtractTextAsync(
                    resume.FileName, resume.Content);
            }

            // Predict skills
            analysis.SkillMatches = (await _mlPredictionService.PredictSkillsAsync(resume.Content)).ToList();
            analysis.ExtractedSkills = analysis.SkillMatches
                .Where(s => s.ConfidenceScore > 0.65)
                .Select(s => s.SkillName)
                .ToList();

            // Extract certifications
            analysis.Certifications = await ExtractCertificationsAsync(resume.Content);

            // Extract education
            analysis.Education = ExtractEducation(resume.Content);

            // Extract work experiences
            analysis.WorkExperiences = ExtractWorkExperiences(resume.Content);

            // Calculate scores
            analysis.CategoryScores = CalculateCategoryScores(analysis);
            analysis.OverallScore = CalculateOverallScore(analysis);

            // Generate recommendations
            analysis.Recommendations = await GenerateRecommendationsAsync(analysis);

            _logger.LogInformation("Analysis completed for resume: {ResumeId} with score: {Score}", 
                resume.Id, analysis.OverallScore);

            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing resume: {ResumeId}", resume.Id);
            throw;
        }
    }

    public async Task<ResumeAnalysisResult> AnalyzeResumeAgainstJobAsync(Resume resume, string jobDescription)
    {
        _logger.LogInformation("Analyzing resume against job description");

        var baseAnalysis = await AnalyzeResumeAsync(resume);

        // Match skills against job requirements
        var jobSkills = await _mlPredictionService.PredictSkillsAsync(jobDescription);
        var matchedSkills = MatchSkills(baseAnalysis.SkillMatches, jobSkills);

        baseAnalysis.SummaryAnalysis = GenerateJobMatchSummary(matchedSkills, baseAnalysis.SkillMatches.Count);

        return baseAnalysis;
    }

    public async Task<double> CalculateAtsScoreAsync(Resume resume)
    {
        _logger.LogInformation("Calculating ATS compatibility score");

        double score = 100.0;

        // Check for formatting issues
        if (string.IsNullOrWhiteSpace(resume.Content))
            score -= 20;

        // Check for required sections
        var requiredSections = new[] { "education", "experience", "skills", "contact" };
        foreach (var section in requiredSections)
        {
            if (!resume.Content.Contains(section, StringComparison.OrdinalIgnoreCase))
            {
                score -= 5;
            }
        }

        // Check content length
        if (resume.Content.Length < 300)
            score -= 10;

        return Math.Max(0, score);
    }

    public async Task<List<string>> GenerateRecommendationsAsync(ResumeAnalysisResult analysis)
    {
        var recommendations = new List<string>();

        if (analysis.OverallScore < 50)
        {
            recommendations.Add("Consider adding more relevant skills and work experience details.");
        }

        if (analysis.Education.Count == 0)
        {
            recommendations.Add("Add your educational background and qualifications.");
        }

        if (analysis.WorkExperiences.Count == 0)
        {
            recommendations.Add("Include your work experience and achievements.");
        }

        if (analysis.ExtractedSkills.Count < 5)
        {
            recommendations.Add("Add more relevant technical and soft skills.");
        }

        if (analysis.Certifications.Count == 0)
        {
            recommendations.Add("Consider adding any professional certifications you possess.");
        }

        if (analysis.OverallScore > 75)
        {
            recommendations.Add("Great resume! Focus on tailoring it to specific job descriptions.");
        }

        return recommendations;
    }

    private async Task<List<string>> ExtractCertificationsAsync(string content)
    {
        var certifications = new List<string>();
        var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var (isCert, confidence) = await _mlPredictionService.PredictCertificationAsync(line);
            if (isCert && confidence > 0.7)
            {
                certifications.Add(line.Trim());
            }
        }

        return certifications;
    }

    private List<string> ExtractEducation(string content)
    {
        var education = new List<string>();
        var degrees = new[] { "B.S.", "B.A.", "M.S.", "M.A.", "Ph.D.", "MBA", "diploma", "degree" };

        var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (degrees.Any(d => line.Contains(d, StringComparison.OrdinalIgnoreCase)))
            {
                education.Add(line.Trim());
            }
        }

        return education;
    }

    private List<WorkExperience> ExtractWorkExperiences(string content)
    {
        var experiences = new List<WorkExperience>();
        // Basic pattern matching - can be enhanced with ML models
        var jobTitlePatterns = new[] { "developer", "engineer", "analyst", "manager", "consultant", "admin" };

        var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (jobTitlePatterns.Any(p => line.Contains(p, StringComparison.OrdinalIgnoreCase)))
            {
                experiences.Add(new WorkExperience
                {
                    JobTitle = line.Trim(),
                    Description = ""
                });
            }
        }

        return experiences;
    }

    private Dictionary<string, double> CalculateCategoryScores(ResumeAnalysisResult analysis)
    {
        return new Dictionary<string, double>
        {
            { "Skills", Math.Min(100, analysis.ExtractedSkills.Count * 10) },
            { "Experience", Math.Min(100, analysis.WorkExperiences.Count * 25) },
            { "Education", Math.Min(100, analysis.Education.Count * 50) },
            { "Certifications", Math.Min(100, analysis.Certifications.Count * 25) },
        };
    }

    private double CalculateOverallScore(ResumeAnalysisResult analysis)
    {
        var scores = analysis.CategoryScores.Values.ToList();
        return scores.Count > 0 ? scores.Average() : 0.0;
    }

    private List<(string Skill, bool Matched)> MatchSkills(
        List<SkillMatch> resumeSkills, 
        SkillMatch[] jobSkills)
    {
        var matched = new List<(string, bool)>();

        foreach (var jobSkill in jobSkills)
        {
            bool isMatched = resumeSkills.Any(rs => 
                rs.SkillName.Equals(jobSkill.SkillName, StringComparison.OrdinalIgnoreCase));
            matched.Add((jobSkill.SkillName, isMatched));
        }

        return matched;
    }

    private string GenerateJobMatchSummary(List<(string Skill, bool Matched)> matches, int totalSkills)
    {
        int matchedCount = matches.Count(m => m.Matched);
        double matchPercentage = (matchedCount / (double)matches.Count) * 100;

        return $"Resume matches {matchPercentage:F1}% of required skills. " +
               $"Found {matchedCount} out of {matches.Count} specified skills.";
    }
}
