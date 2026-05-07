# Smart Resume Analyzer - Getting Started Guide

## Project Overview

Smart Resume Analyzer is a comprehensive .NET 8.0 application that leverages ML.NET to analyze resumes intelligently, extract key information, and provide actionable recommendations for improvement.

## Quick Start

### 1. Prerequisites
- .NET 8.0 SDK or higher
- Visual Studio 2022, Visual Studio Code, or Rider
- Git (optional, for version control)

### 2. Building the Project

```bash
# Navigate to the project directory
cd SmartResumeAnalyzer

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

### 3. First Run

When you run the application, you'll see an interactive menu:

```
========== Smart Resume Analyzer ==========
1. Analyze Resume
2. Analyze Resume Against Job Description
3. View Resume Analysis
4. Search Resumes
5. Calculate ATS Score
6. Exit
```

### 4. Using the Features

#### Analyzing a Resume
1. Select option "1"
2. Provide the full path to your resume file (PDF, DOCX, or TXT)
3. The app will analyze and display:
   - Overall score (0-100)
   - Extracted skills with confidence scores
   - Detected education and work experience
   - Certifications found
   - Personalized recommendations

#### Job Matching
1. Select option "2"
2. Provide resume file path
3. Provide job description (as text or file path)
4. Get skill-match percentage and detailed comparison

#### ATS Compatibility
1. Select option "5"
2. Enter resume ID (from previous analysis)
3. Get ATS compatibility score with feedback

## Configuration

### appsettings.json

Customize behavior in `appsettings.json`:

```json
{
  "ResumeAnalyzer": {
    "ModelPath": "./Models/trained_model.zip",
    "MinConfidenceScore": 0.65,
    "MaxResumeLength": 10000
  }
}
```

### Adding Custom Skills

Edit `Data/SkillDatabase.cs` to add:
- New skill categories
- Domain-specific certifications
- Company-specific skills

Example:
```csharp
"Blockchain", new[]
{
    "Solidity", "Ethereum", "Web3.js", "Truffle"
}
```

## Architecture Overview

```
├── Models/          - Data structures and classes
├── Services/        - Core business logic
├── ML/             - ML.NET models and training
├── Data/           - Skill databases and lookups
└── Utils/          - Helper functions and utilities
```

### Key Components

| Component | Purpose |
|-----------|---------|
| `MLPredictionService` | ML.NET predictions and model management |
| `ResumeAnalysisService` | Core analysis engine |
| `TextExtractionService` | Extract text from PDF/DOCX/TXT |
| `ResumeStorageService` | Persist and retrieve analyses |

## Extending the System

### Adding a New File Format

1. Update `TextExtractionService.ExtractTextAsync()`:
```csharp
".xyz" => await ExtractFromXyzAsync(fileContent)
```

2. Implement the extraction method:
```csharp
public async Task<string> ExtractFromXyzAsync(byte[] fileContent)
{
    // Implementation
}
```

### Training a Custom ML Model

Use the `TrainSkillsModelAsync()` method:
```csharp
var trainingData = new List<(string Text, string[] Labels)>
{
    ("C# and .NET developer", new[] { "C#", ".NET" }),
    // More examples...
};

await mlService.TrainSkillsModelAsync(trainingData);
mlService.SaveModel("./Models/custom_model.zip");
```

### Adding Custom Scoring Logic

Modify `ResumeAnalysisService.CalculateCategoryScores()` to adjust weights and thresholds.

## Testing

### Manual Testing Checklist

- [ ] Load a sample resume (TXT format for simplicity)
- [ ] Verify skill extraction
- [ ] Check job description matching
- [ ] Test ATS score calculation
- [ ] Verify recommendations are relevant

### Sample Resume for Testing

Create a `test_resume.txt`:
```
John Doe
Email: john@example.com
Phone: 555-0123

Education:
Bachelor of Science in Computer Science
University of Example, 2020

Experience:
Senior Developer at Tech Corp (2021-Present)
- Developed C# and .NET applications
- Led team of 5 developers
- Implemented Azure DevOps pipeline

Skills:
C#, .NET, ASP.NET, Azure, SQL Server, Git

Certifications:
Microsoft Certified: Azure Developer Associate
```

## Common Issues

### Issue: Model file not found
**Solution**: Train a model first or ensure model path is correct in appsettings.json

### Issue: Text extraction failing for PDF
**Solution**: Install iTextSharp: `dotnet add package iTextSharp`

### Issue: No skills detected
**Solution**: Check that skill names match the database in `SkillDatabase.cs`

## Performance Optimization

- [ ] Implement caching for repeated analyses
- [ ] Batch process multiple resumes
- [ ] Use async/await throughout
- [ ] Profile with dotTrace or VS profiler

## Security Considerations

- Validate file uploads (size, format, content)
- Sanitize extracted text before processing
- Secure API keys and connection strings
- Implement rate limiting for API endpoints

## Next Steps

1. ✅ Run the application
2. ✅ Test with sample resume
3. ✅ Customize skill database
4. ✅ Train custom ML models
5. ✅ Deploy to production

## Resources

- [ML.NET Documentation](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [.NET 8.0 Guide](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [iTextSharp Documentation](https://github.com/itext/itextsharp)

## Support

Check the console logs for detailed error messages. Enable debug logging by modifying `appsettings.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Debug"
  }
}
```
