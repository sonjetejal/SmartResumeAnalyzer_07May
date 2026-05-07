using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartResumeAnalyzer.Utils;

public static class TextPreprocessor
{
    /// <summary>
    /// Cleans and normalizes text for analysis
    /// </summary>
    public static string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLower();

        // Remove extra whitespace
        text = Regex.Replace(text, @"\s+", " ").Trim();

        // Remove special characters except hyphens and underscores
        text = Regex.Replace(text, @"[^\w\s\-]", " ");

        return text;
    }

    /// <summary>
    /// Tokenizes text into words
    /// </summary>
    public static string[] Tokenize(string text)
    {
        var cleaned = CleanText(text);
        return cleaned.Split(new[] { ' ', '\t', '\n', '\r' }, 
            StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Removes common stop words
    /// </summary>
    public static string[] RemoveStopWords(string[] tokens)
    {
        var stopWords = new HashSet<string>
        {
            "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for",
            "of", "with", "by", "from", "as", "is", "was", "are", "be", "been",
            "have", "has", "had", "do", "does", "did", "will", "would", "could",
            "should", "may", "might", "must", "can"
        };

        return tokens.Where(t => !stopWords.Contains(t) && t.Length > 2).ToArray();
    }

    /// <summary>
    /// Extracts email addresses from text
    /// </summary>
    public static string[] ExtractEmails(string text)
    {
        var pattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
        var matches = Regex.Matches(text, pattern);
        return matches.Cast<Match>().Select(m => m.Value).ToArray();
    }

    /// <summary>
    /// Extracts phone numbers from text
    /// </summary>
    public static string[] ExtractPhoneNumbers(string text)
    {
        var pattern = @"(\+?1[-.\s]?)?\(?([0-9]{3})\)?[-.\s]?([0-9]{3})[-.\s]?([0-9]{4})";
        var matches = Regex.Matches(text, pattern);
        return matches.Cast<Match>().Select(m => m.Value).ToArray();
    }

    /// <summary>
    /// Extracts years from text
    /// </summary>
    public static int[] ExtractYears(string text)
    {
        var pattern = @"\b(19|20)\d{2}\b";
        var matches = Regex.Matches(text, pattern);
        return matches.Cast<Match>()
            .Select(m => int.Parse(m.Value))
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Calculates similarity between two strings (0-1)
    /// </summary>
    public static double CalculateSimilarity(string str1, string str2)
    {
        if (str1 == str2) return 1.0;

        var tokens1 = Tokenize(str1).ToHashSet();
        var tokens2 = Tokenize(str2).ToHashSet();

        int intersection = tokens1.Intersect(tokens2).Count();
        int union = tokens1.Union(tokens2).Count();

        return union == 0 ? 0 : (double)intersection / union;
    }
}

public static class SkillMatcher
{
    /// <summary>
    /// Matches text against known skills with fuzzy matching
    /// </summary>
    public static List<(string Skill, double Match)> FindSkillMatches(
        string text, 
        IEnumerable<string> knownSkills,
        double threshold = 0.7)
    {
        var matches = new List<(string, double)>();
        var tokens = TextPreprocessor.Tokenize(text);

        foreach (var skill in knownSkills)
        {
            var skillTokens = TextPreprocessor.Tokenize(skill);

            foreach (var token in tokens)
            {
                var similarity = TextPreprocessor.CalculateSimilarity(token, skill);
                
                if (similarity >= threshold)
                {
                    matches.Add((skill, similarity));
                }
            }
        }

        return matches.GroupBy(m => m.Item1)
            .Select(g => (g.Key, g.Max(x => x.Item2)))
            .OrderByDescending(x => x.Item2)
            .ToList();
    }

    /// <summary>
    /// Checks if text contains skill variations
    /// </summary>
    public static bool HasSkillVariations(string text, string skill)
    {
        var variations = GetSkillVariations(skill);
        return variations.Any(v => text.Contains(v, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets common variations of a skill name
    /// </summary>
    private static string[] GetSkillVariations(string skill)
    {
        var variations = new List<string> { skill };

        var skillVariationMap = new Dictionary<string, string[]>
        {
            { "c#", new[] { "csharp", "c sharp" } },
            { "javascript", new[] { "js", "javascript" } },
            { "typescript", new[] { "ts", "typescript" } },
            { "python", new[] { "py", "python" } },
            { ".net", new[] { ".net", "dotnet", "dot net" } },
            { "sql", new[] { "sql", "t-sql", "plsql" } },
            { "html", new[] { "html", "html5", "html 5" } },
            { "css", new[] { "css", "css3", "css 3" } },
        };

        if (skillVariationMap.TryGetValue(skill.ToLower(), out var skVar))
        {
            variations.AddRange(skVar);
        }

        return variations.ToArray();
    }
}

public static class RecommendationGenerator
{
    /// <summary>
    /// Generates actionable recommendations based on analysis
    /// </summary>
    public static List<string> GenerateRecommendations(
        double overallScore,
        int skillCount,
        int experienceCount,
        int educationCount,
        int certificationCount)
    {
        var recommendations = new List<string>();

        // Score-based recommendations
        if (overallScore < 30)
        {
            recommendations.Add("Add more relevant work experience and skills to enhance your profile.");
            recommendations.Add("Include at least 2-3 years of relevant work experience.");
        }
        else if (overallScore < 50)
        {
            recommendations.Add("Consider adding more technical skills and certifications.");
            recommendations.Add("Expand on your work experience with specific achievements and metrics.");
        }
        else if (overallScore < 70)
        {
            recommendations.Add("Add quantifiable achievements to your work experience (e.g., 'Increased efficiency by 30%').");
            recommendations.Add("Include relevant industry certifications to strengthen your profile.");
        }

        // Skill-based recommendations
        if (skillCount < 5)
        {
            recommendations.Add($"You've listed {skillCount} skills. Aim for 8-12 relevant skills.");
        }
        else if (skillCount > 20)
        {
            recommendations.Add("Consider including only the most relevant skills to avoid overwhelming employers.");
        }

        // Experience-based recommendations
        if (experienceCount == 0)
        {
            recommendations.Add("Include your work history with job titles, companies, and dates.");
        }
        else if (experienceCount == 1)
        {
            recommendations.Add("Add more work experience entries for a more comprehensive profile.");
        }

        // Education-based recommendations
        if (educationCount == 0)
        {
            recommendations.Add("Include your educational qualifications (degree, university, graduation year).");
        }

        // Certification-based recommendations
        if (certificationCount == 0)
        {
            recommendations.Add("Add professional certifications (AWS, Azure, Scrum Master, etc.) if applicable.");
        }
        else if (certificationCount > 5)
        {
            recommendations.Add("List your 5 most relevant and recent certifications for maximum impact.");
        }

        // Format recommendations
        recommendations.Add("Ensure consistent formatting and proper grammar throughout your resume.");
        recommendations.Add("Use clear section headings (Education, Experience, Skills, Certifications).");
        recommendations.Add("Keep your resume to 1-2 pages for better ATS compatibility.");

        return recommendations;
    }
}
