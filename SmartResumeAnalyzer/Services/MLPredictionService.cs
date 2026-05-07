using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Extensions.Logging;
using SmartResumeAnalyzer.Models;

namespace SmartResumeAnalyzer.Services;

public interface IMLPredictionService
{
    Task<SkillMatch[]> PredictSkillsAsync(string resumeText);
    Task<(bool IsCertification, float Confidence)> PredictCertificationAsync(string text);
    Task TrainSkillsModelAsync(List<(string Text, string[] Labels)> trainingData);
    void SaveModel(string modelPath);
    void LoadModel(string modelPath);
}

public class MLPredictionService : IMLPredictionService
{
    private readonly MLContext _mlContext;
    private readonly ILogger<MLPredictionService> _logger;
    private ITransformer? _trainedModel;

    public MLPredictionService(ILogger<MLPredictionService> logger)
    {
        _mlContext = new MLContext(seed: 0);
        _logger = logger;
    }

    public async Task<SkillMatch[]> PredictSkillsAsync(string resumeText)
    {
        _logger.LogInformation("Starting skill prediction analysis");
        
        if (_trainedModel == null)
        {
            _logger.LogWarning("Model not trained or loaded. Using default skill extraction.", null);
            return ExtractSkillsManual(resumeText);
        }

        try
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MLInputData, MLOutputData>(_trainedModel);
            var input = new MLInputData { Text = resumeText };
            var output = predictionEngine.Predict(input);

            return ProcessPredictions(resumeText, output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during skill prediction");
            return ExtractSkillsManual(resumeText);
        }
    }

    public async Task<(bool IsCertification, float Confidence)> PredictCertificationAsync(string text)
    {
        _logger.LogInformation("Analyzing text for certifications");
        
        try
        {
            // Pattern matching for certifications
            var certificationPatterns = new[]
            {
                "AWS", "Azure", "GCP", "Kubernetes", "Docker",
                "PMP", "CISSP", "CEH", "OSCP", "Scrum",
                "Microsoft Certified", "Google Cloud Certified"
            };

            bool isCertification = certificationPatterns.Any(p => 
                text.Contains(p, StringComparison.OrdinalIgnoreCase));
            
            float confidence = isCertification ? 0.95f : 0.1f;

            return (isCertification, confidence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during certification prediction");
            return (false, 0.0f);
        }
    }

    public async Task TrainSkillsModelAsync(List<(string Text, string[] Labels)> trainingData)
    {
        _logger.LogInformation("Starting model training with {Count} data points", trainingData.Count);

        try
        {
            // Convert training data to IDataView
            var trainData = _mlContext.Data.LoadFromEnumerable(
                trainingData.Select(x => new MLInputData { Text = x.Text })
            );

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Text")
                .Append(_mlContext.Transforms.NormalizeMinMax("Features", "Features"))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Score", 
                    featureColumnName: "Features"));

            _trainedModel = pipeline.Fit(trainData);
            _logger.LogInformation("Model training completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during model training");
            throw;
        }
    }

    public void SaveModel(string modelPath)
    {
        if (_trainedModel == null)
        {
            _logger.LogWarning("No trained model to save");
            return;
        }

        try
        {
            var directory = Path.GetDirectoryName(modelPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            _mlContext.Model.Save(_trainedModel, null, modelPath);
            _logger.LogInformation("Model saved to {Path}", modelPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving model");
            throw;
        }
    }

    public void LoadModel(string modelPath)
    {
        try
        {
            if (!File.Exists(modelPath))
            {
                _logger.LogWarning("Model file not found at {Path}", modelPath);
                return;
            }

            using var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _trainedModel = _mlContext.Model.Load(stream, out var inputSchema);
            _logger.LogInformation("Model loaded from {Path}", modelPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading model from {Path}", modelPath);
            throw;
        }
    }

    private SkillMatch[] ProcessPredictions(string resumeText, MLOutputData predictions)
    {
        var skills = new List<SkillMatch>();
        
        // Simple extraction based on predictions
        var tokens = resumeText.Split(new[] { ' ', '\n', '\r', ',', '.', ':', ';' }, 
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens.Distinct())
        {
            var match = new SkillMatch
            {
                SkillName = token,
                ConfidenceScore = predictions.Score * 0.8,
                Category = "Technical",
                Frequency = tokens.Count(t => t.Equals(token, StringComparison.OrdinalIgnoreCase))
            };

            if (match.ConfidenceScore > 0.6)
            {
                skills.Add(match);
            }
        }

        return skills.OrderByDescending(s => s.ConfidenceScore).ToArray();
    }

    private SkillMatch[] ExtractSkillsManual(string resumeText)
    {
        var technicalSkills = new[] 
        {
            "C#", "Python", "Java", "JavaScript", "TypeScript", ".NET", "ASP.NET",
            "SQL", "MongoDB", "Docker", "Kubernetes", "Azure", "AWS",
            "React", "Angular", "Node.js", "Machine Learning", "API", "REST"
        };

        var skills = new List<SkillMatch>();

        foreach (var skill in technicalSkills)
        {
            int count = CountOccurrences(resumeText, skill);
            if (count > 0)
            {
                skills.Add(new SkillMatch
                {
                    SkillName = skill,
                    ConfidenceScore = Math.Min(0.95, 0.5 + (count * 0.1)),
                    Category = "Technical",
                    Frequency = count
                });
            }
        }

        return skills.OrderByDescending(s => s.Frequency).ToArray();
    }

    private int CountOccurrences(string text, string value)
    {
        int count = 0;
        int start = 0;
        
        while ((start = text.IndexOf(value, start, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            count++;
            start += value.Length;
        }

        return count;
    }
}
