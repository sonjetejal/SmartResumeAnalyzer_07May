# Project Documentation - Smart Resume Analyzer

## Table of Contents
1. [Overview](#overview)
2. [Technology Stack](#technology-stack)
3. [Project Structure](#project-structure)
4. [Installation & Setup](#installation--setup)
5. [Usage Guide](#usage-guide)
6. [API Documentation](#api-documentation)
7. [Machine Learning Components](#machine-learning-components)
8. [Best Practices](#best-practices)
9. [Troubleshooting](#troubleshooting)
10. [Future Roadmap](#future-roadmap)

---

## Overview

**Smart Resume Analyzer** is a sophisticated .NET 8.0 application that uses ML.NET to intelligently analyze resumes, extract key information, and provide data-driven recommendations for improvement.

### Key Capabilities
- **Intelligent Resume Analysis**: Extract skills, education, experience, and certifications
- **ML-Powered Predictions**: Use machine learning models for pattern recognition
- **Job Matching**: Compare resumes against job descriptions
- **ATS Compatibility**: Check resume formatting for Applicant Tracking Systems
- **Skill Intelligence**: Categorize and match skills intelligently
- **Scoring & Recommendations**: Generate actionable improvement suggestions

---

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Runtime** | .NET | 8.0+ |
| **Language** | C# | 12.0 |
| **ML Framework** | ML.NET | 3.0.0 |
| **Configuration** | Microsoft.Extensions.Configuration | 8.0.0 |
| **Logging** | Serilog | 3.1.1 |
| **DI Framework** | Microsoft.Extensions.DependencyInjection | 8.0.0 |
| **File Handling** | iTextSharp | 5.5.13.3 |

---

## Project Structure

```
SmartResumeAnalyzer/
│
├── 📄 Program.cs                     # Application entry point
├── 📄 appsettings.json              # Configuration settings
├── 📄 SmartResumeAnalyzer.csproj   # Project file
├── 📄 .gitignore                    # Git ignore rules
│
├── 📁 Models/                       # Data models
│   ├── ResumeModel.cs              # Resume and analysis entities
│   └── MLModel.cs                  # ML.NET model classes
│
├── 📁 Services/                     # Business logic services
│   ├── IMLPredictionService.cs     # ML service interface
│   ├── MLPredictionService.cs      # ML implementation
│   ├── IResumeAnalysisService.cs   # Analysis interface
│   ├── ResumeAnalysisService.cs    # Analysis implementation
│   ├── ITextExtractionService.cs   # Text extraction interface
│   ├── TextExtractionService.cs    # Text extraction implementation
│   ├── IResumeStorageService.cs    # Storage interface
│   └── ResumeStorageService.cs     # Storage implementation
│
├── 📁 ML/                           # Machine learning models
│   └── Models/ (generated)          # Trained ML.NET models
│
├── 📁 Data/                         # Data and knowledge bases
│   ├── SkillDatabase.cs            # Comprehensive skill categorization
│   └── SampleTrainingData.cs       # Example data for training
│
├── 📁 Utils/                        # Utility functions
│   ├── TextProcessing.cs           # Text processing utilities
│   ├── Constants.cs                # Application constants
│   └── AnalysisConstants.cs        # Analysis-specific constants
│
├── 📄 README.md                     # Overview and quick start
├── 📄 GETTING_STARTED.md            # Detailed getting started guide
├── 📄 ARCHITECTURE.md               # Architecture and design patterns
└── 📄 PROJECT_DOCUMENTATION.md      # This file

```

---

## Installation & Setup

### Prerequisites
```bash
# Check .NET version
dotnet --version
# Should output: 8.0.0 or higher
```

### Step-by-Step Setup

1. **Clone/Download the Project**
   ```bash
   cd SmartResumeAnalyzer
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Project**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

### Configuration

Edit `appsettings.json` for production settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ResumeAnalyzer": {
    "ModelPath": "./Models/trained_model.zip",
    "MinConfidenceScore": 0.65,
    "MaxResumeLength": 10000,
    "SupportedFormats": ["pdf", "docx", "txt"]
  }
}
```

---

## Usage Guide

### Menu Flow

```
Main Menu
├── Option 1: Analyze Resume
│   ├── Input: Resume file path
│   └── Output: Detailed analysis with scores
├── Option 2: Analyze Against Job
│   ├── Input: Resume + Job description
│   └── Output: Match percentage and recommendations
├── Option 3: View Analysis
│   ├── Input: Resume ID
│   └── Output: Previous analysis results
├── Option 4: Search Resumes
│   ├── Input: Search query
│   └── Output: Matching resumes
├── Option 5: Calculate ATS Score
│   ├── Input: Resume ID
│   └── Output: ATS compatibility percentage
└── Option 6: Exit
```

### Example Workflow

```csharp
// 1. Create a resume object
var resume = new Resume
{
    FileName = "john_doe_resume.txt",
    Content = resumeText,
    FileFormat = ".txt"
};

// 2. Analyze the resume
var analysis = await analysisService.AnalyzeResumeAsync(resume);

// 3. Access results
Console.WriteLine($"Overall Score: {analysis.OverallScore}");
foreach (var skill in analysis.ExtractedSkills)
{
    Console.WriteLine($"  Skill: {skill}");
}

// 4. Save for later
await storageService.SaveResumeAsync(resume);
```

---

## API Documentation

### ResumeAnalysisService

#### AnalyzeResumeAsync
```csharp
Task<ResumeAnalysisResult> AnalyzeResumeAsync(Resume resume)
```
**Purpose**: Complete resume analysis including skill extraction, education/experience detection, and scoring.

**Parameters**:
- `resume`: Resume object containing file name, content, and format

**Returns**: ResumeAnalysisResult with complete analysis

**Example**:
```csharp
var result = await service.AnalyzeResumeAsync(resume);
Console.WriteLine($"Skills found: {result.ExtractedSkills.Count}");
```

#### AnalyzeResumeAgainstJobAsync
```csharp
Task<ResumeAnalysisResult> AnalyzeResumeAgainstJobAsync(
    Resume resume, 
    string jobDescription)
```
**Purpose**: Compare resume against job requirements.

**Parameters**:
- `resume`: Resume to analyze
- `jobDescription`: Job requirements text

**Returns**: Analysis with job-specific match results

#### CalculateAtsScoreAsync
```csharp
Task<double> CalculateAtsScoreAsync(Resume resume)
```
**Purpose**: Evaluate ATS compatibility.

**Returns**: Score from 0-100

#### GenerateRecommendationsAsync
```csharp
Task<List<string>> GenerateRecommendationsAsync(
    ResumeAnalysisResult analysis)
```
**Purpose**: Generate actionable recommendations.

**Returns**: List of improvement suggestions

### MLPredictionService

#### PredictSkillsAsync
```csharp
Task<SkillMatch[]> PredictSkillsAsync(string resumeText)
```
**Purpose**: Extract and score skills from text.

**Returns**: Array of SkillMatch objects with confidence scores

#### PredictCertificationAsync
```csharp
Task<(bool IsCertification, float Confidence)> 
    PredictCertificationAsync(string text)
```
**Purpose**: Detect certifications in text.

#### TrainSkillsModelAsync
```csharp
Task TrainSkillsModelAsync(
    List<(string Text, string[] Labels)> trainingData)
```
**Purpose**: Train custom ML model.

**Parameters**:
- `trainingData`: List of text samples with skill labels

---

## Machine Learning Components

### ML.NET Architecture

```
Training Data
    ↓
Text Featurization (Convert text to features)
    ↓
Normalization (Scale features)
    ↓
SDCA Regression (Train model)
    ↓
Trained Model
    ↓
Save to ZIP file
    ↓
Load in Prediction Service
    ↓
Make Predictions
```

### Model Training

```csharp
var trainingData = new List<(string, string[])>
{
    ("C# and .NET developer", new[] { "C#", ".NET" }),
    ("JavaScript and React expert", new[] { "JavaScript", "React" })
};

await mlService.TrainSkillsModelAsync(trainingData);
mlService.SaveModel("./Models/skills_model.zip");
```

### Model Prediction

```csharp
await mlService.LoadModel("./Models/skills_model.zip");
var skills = await mlService.PredictSkillsAsync(resumeText);

foreach (var skill in skills)
{
    Console.WriteLine($"{skill.SkillName}: {skill.ConfidenceScore:P2}");
}
```

---

## Best Practices

### 1. Resume Format
✅ **Good Format**:
```
John Doe
john@email.com | (555) 123-4567

EDUCATION
Bachelor of Science in Computer Science
University Name, 2020

EXPERIENCE
Senior Developer - Company (2020-Present)
- Developed C# applications
- Led team of 5 developers

SKILLS
C#, .NET, Azure, SQL Server

CERTIFICATIONS
Microsoft Certified: Azure Developer
```

❌ **Avoid**:
- Images and graphics (breaks ATS)
- Unusual fonts and colors
- Tables without text alternatives
- Inconsistent formatting

### 2. Skill Naming
✅ Use standardized names from SkillDatabase
❌ Avoid variations like "c#, c-sharp, csharp"

### 3. Content Organization
✅ Include: Education, Experience, Skills, Certifications
❌ Omit: Personal photos, references, salary expectations

### 4. Code Best Practices
```csharp
// ✅ Good: Async operations
await analysisService.AnalyzeResumeAsync(resume);

// ✅ Good: Dependency injection
public ResumeAnalysisService(
    IMLPredictionService mlService,
    ILogger<ResumeAnalysisService> logger)

// ❌ Avoid: Synchronous blocking
analysis = analysisService.AnalyzeResume(resume).Result;
```

---

## Troubleshooting

### Common Issues

#### Issue: "Model file not found"
```
Error: The model path './Models/trained_model.zip' does not exist
```
**Solution**:
1. Train a model: `await mlService.TrainSkillsModelAsync(trainingData);`
2. Save it: `mlService.SaveModel("./Models/model.zip");`
3. Update appsettings.json with correct path

#### Issue: "Text extraction failing for PDF"
```
Error: Cannot extract text from PDF file
```
**Solution**:
1. Ensure iTextSharp is installed: `dotnet add package iTextSharp`
2. Verify PDF is not corrupted
3. Use a sample PDF to test

#### Issue: "No skills detected"
```
Observation: ExtractedSkills list is empty
```
**Solution**:
1. Check skill names match SkillDatabase.cs
2. Verify resume contains standard skill names
3. Check MinConfidenceScore threshold in appsettings.json

#### Issue: "Out of memory with large resume"
```
Error: OutOfMemoryException
```
**Solution**:
1. Increase heap size: `dotnet run --configuration Release`
2. Reduce MaxResumeLength in settings
3. Process resumes in batches

### Debugging

Enable debug logging:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

Check logs for detailed error messages and stack traces.

---

## Future Roadmap

### Version 1.1 (Q2 2026)
- [ ] Web API (ASP.NET Core endpoints)
- [ ] Enhanced PDF parsing
- [ ] Interview question generation
- [ ] Resume templates

### Version 1.2 (Q3 2026)
- [ ] Batch processing
- [ ] Database integration (SQL Server/PostgreSQL)
- [ ] Advanced NLP models
- [ ] Competitive resume analysis

### Version 2.0 (Q4 2026)
- [ ] Web UI (React)
- [ ] Job board API integrations
- [ ] Salary prediction
- [ ] Interview preparation modules
- [ ] Team collaboration features

---

## Contributing

To extend the project:

1. **Add Skills**: Edit `SkillDatabase.cs`
2. **Add File Formats**: Extend `TextExtractionService.cs`
3. **Improve Scoring**: Modify `ResumeAnalysisService.cs`
4. **Train Models**: Use `MLPredictionService.TrainSkillsModelAsync()`

---

## License & Support

**Created**: March 28, 2026
**Version**: 1.0.0
**Status**: Production Ready

For questions or issues, refer to:
- README.md - Quick start
- GETTING_STARTED.md - Detailed guide
- ARCHITECTURE.md - Technical deep dive
- Console logs - Error details

---

## Resources

- [ML.NET Official Docs](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [.NET 8.0 Guide](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [C# Language Features](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [iTextSharp Documentation](https://github.com/itext/itextsharp)
