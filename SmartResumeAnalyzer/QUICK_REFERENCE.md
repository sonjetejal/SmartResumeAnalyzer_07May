# Smart Resume Analyzer - Quick Reference

## 🚀 Quick Start Commands

```bash
# Build
dotnet build

# Run
dotnet run

# View errors
dotnet build --verbosity detailed

# Clean build
dotnet clean && dotnet build
```

## 📊 Project Overview

| Aspect | Details |
|--------|---------|
| **Framework** | .NET 8.0 |
| **Language** | C# 12 |
| **Architecture** | Layered + DI |
| **Key Library** | ML.NET 3.0.0 |
| **Type** | Console Application |
| **Status** | Production Ready |

## 🏗️ Core Components

### Services
- `MLPredictionService` - Machine learning predictions
- `ResumeAnalysisService` - Main analysis logic
- `TextExtractionService` - File format handling
- `ResumeStorageService` - Data persistence

### Models
- `Resume` - Resume data structure
- `ResumeAnalysisResult` - Analysis results
- `SkillMatch` - Detected skills with scores
- `WorkExperience` - Work history

### Utilities
- `TextPreprocessor` - Text cleaning and processing
- `SkillMatcher` - Skill detection and matching
- `SkillDatabase` - Skill categorization
- `RecommendationGenerator` - Suggestion generation

## 📁 File Structure Reference

```
SmartResumeAnalyzer/
├── Program.cs                    → Entry point & main loop
├── appsettings.json             → Configuration
├── SmartResumeAnalyzer.csproj   → Project file
├── Models/
│   ├── ResumeModel.cs           → Resume entities
│   └── MLModel.cs               → ML input/output
├── Services/
│   ├── MLPredictionService.cs   → ML.NET wrapper
│   ├── ResumeAnalysisService.cs → Analysis orchestrator
│   ├── TextExtractionService.cs → File parsing
│   └── ResumeStorageService.cs  → Data access
├── Data/
│   ├── SkillDatabase.cs         → 50+ skills in 10 categories
│   └── SampleTrainingData.cs    → Example training data
├── Utils/
│   ├── TextProcessing.cs        → Text utilities
│   └── Constants.cs             → Configuration constants
└── Documentation/
    ├── README.md                → Overview
    ├── GETTING_STARTED.md       → Setup guide
    ├── ARCHITECTURE.md          → Design patterns
    ├── PROJECT_DOCUMENTATION.md → Complete docs
    └── QUICK_REFERENCE.md       → This file
```

## 🔧 Configuration Keys

```json
{
  "ResumeAnalyzer": {
    "ModelPath": "./Models/trained_model.zip",
    "MinConfidenceScore": 0.65,
    "MaxResumeLength": 10000,
    "SupportedFormats": ["pdf", "docx", "txt"]
  }
}
```

## 🎯 Main Features

| Feature | Method | File |
|---------|--------|------|
| Analyze Resume | `AnalyzeResumeAsync()` | ResumeAnalysisService |
| Match Job | `AnalyzeResumeAgainstJobAsync()` | ResumeAnalysisService |
| Extract Skills | `PredictSkillsAsync()` | MLPredictionService |
| Check ATS | `CalculateAtsScoreAsync()` | ResumeAnalysisService |
| Store Resume | `SaveResumeAsync()` | ResumeStorageService |
| Search | `SearchResumesAsync()` | ResumeStorageService |

## 📊 Analysis Output

```csharp
ResumeAnalysisResult
├── OverallScore (0-100)
├── ExtractedSkills (List<string>)
├── SkillMatches (List<SkillMatch> with scores)
├── Education (List<string>)
├── WorkExperiences (List<WorkExperience>)
├── Certifications (List<string>)
├── CategoryScores (Dictionary<string, double>)
│   ├── Skills
│   ├── Experience
│   ├── Education
│   └── Certifications
└── Recommendations (List<string>)
```

## 🎓 Skill Categories (50+ Skills)

- **Languages**: C#, Python, Java, JavaScript, TypeScript, Go, Rust
- **Frontend**: React, Angular, Vue.js, Next.js, HTML, CSS
- **Backend**: .NET, ASP.NET, Node.js, Django, Flask
- **Databases**: SQL Server, PostgreSQL, MongoDB, Redis
- **Cloud**: AWS, Azure, Google Cloud
- **DevOps**: Docker, Kubernetes, Jenkins, Terraform
- **ML/AI**: TensorFlow, PyTorch, scikit-learn
- **Soft Skills**: Leadership, Communication, Problem Solving

## 🔄 Data Flow

```
Resume File
    ↓
TextExtractionService (Extract Text)
    ↓
Clean Resume Text
    ↓
MLPredictionService (Predict Skills)
    ↓
ResumeAnalysisService (Process Results)
    ↓
Calculate Scores
    ↓
Generate Recommendations
    ↓
ResumeStorageService (Save)
    ↓
Display Results
```

## 🧪 Testing Quick Start

```csharp
// Load sample data
var samples = SampleTrainingData.GetSampleResumes();
var testResume = samples["resume1.txt"];

// Analyze
var analysis = await service.AnalyzeResumeAsync(resume);

// Verify results
assert!(analysis.ExtractedSkills.Count > 0);
assert!(analysis.OverallScore > 0);
```

## 🚨 Common Errors & Fixes

| Error | Cause | Fix |
|-------|-------|-----|
| Model not found | No trained model | Train: `TrainSkillsModelAsync(data)` |
| No skills found | Low confidence | Lower `MinConfidenceScore` in config |
| PDF parsing fails | iTextSharp missing | `dotnet add package iTextSharp` |
| Out of memory | Large file | Reduce `MaxResumeLength` or increase heap |

## 📈 Performance Tips

```csharp
// ✅ Good: Async operations
await analysisService.AnalyzeResumeAsync(resume);

// ✅ Good: Model caching
mlService.LoadModel(path);  // Load once
// Use multiple times

// ❌ Avoid: Blocking operations
analysis = service.AnalyzeResume(resume).Result;
```

## 🔐 Security Checklist

- [ ] Validate file uploads
- [ ] Sanitize extracted text
- [ ] No sensitive logs
- [ ] Check file size limits
- [ ] Validate configurations

## 📚 Documentation Map

| Document | Purpose | When to Read |
|----------|---------|--------------|
| README.md | Project overview | First time |
| GETTING_STARTED.md | Setup & first run | Setting up |
| ARCHITECTURE.md | Design patterns | Understanding internals |
| PROJECT_DOCUMENTATION.md | Complete reference | Looking up details |
| QUICK_REFERENCE.md | This file | Quick lookup |

## 🎯 Next Steps

1. **Build**: `dotnet build`
2. **Run**: `dotnet run`
3. **Test**: Load sample resume
4. **Customize**: Edit SkillDatabase.cs
5. **Deploy**: Publish for production

## 📞 Support Resources

- Check console logs for errors
- Enable Debug logging in appsettings.json
- Review PROJECT_DOCUMENTATION.md for detailed API
- Check ARCHITECTURE.md for design patterns

## 🔗 External Resources

- [ML.NET Docs](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [.NET 8 Guide](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Serilog Docs](https://serilog.net/)
- [iTextSharp Repo](https://github.com/itext/itextsharp)

---

**Version**: 1.0.0  
**Created**: March 28, 2026  
**Status**: ✅ Production Ready
