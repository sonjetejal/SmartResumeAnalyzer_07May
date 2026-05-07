# Smart Resume Analyzer

A comprehensive .NET application for analyzing resumes using ML.NET services. This tool provides intelligent resume analysis, skill extraction, and job matching capabilities.

## Features

- **Resume Analysis**: Analyzes resume content and extracts key information
- **ML.NET Integration**: Leverages machine learning for skill and certification prediction
- **Multiple File Format Support**: Handles PDF, DOCX, and TXT files
- **Job Matching**: Compares resume skills against job requirements
- **ATS Compatibility Score**: Evaluates resume for Applicant Tracking System compatibility
- **Skill Extraction**: Automatically identifies technical and soft skills
- **Recommendations**: Provides actionable suggestions for resume improvement
- **Resume Storage**: Persists analyzed resumes for future reference

## Project Structure

```
SmartResumeAnalyzer/
├── Models/
│   ├── ResumeModel.cs         - Resume and analysis data models
│   └── MLModel.cs             - ML.NET model input/output classes
├── Services/
│   ├── MLPredictionService.cs  - ML.NET prediction and training
│   ├── ResumeAnalysisService.cs - Core analysis logic
│   ├── TextExtractionService.cs - File format extraction
│   └── ResumeStorageService.cs  - Data persistence
├── Program.cs                  - Main application entry point
├── appsettings.json           - Configuration file
└── SmartResumeAnalyzer.csproj - Project file
```

## Requirements

- .NET 8.0 or higher
- Visual Studio 2022 or Visual Studio Code
- Windows/Linux/macOS

## Installation

1. Clone or download the project
2. Open in Visual Studio or VS Code
3. Build the project:
   ```bash
   dotnet build
   ```

## Usage

1. Run the application:
   ```bash
   dotnet run
   ```

2. Select from the menu options:
   - **Analyze Resume**: Upload and analyze a single resume
   - **Analyze Against Job Description**: Match resume against job requirements
   - **View Resume Analysis**: Retrieve previous analysis results
   - **Search Resumes**: Find resumes by keywords
   - **Calculate ATS Score**: Check ATS compatibility

## Configuration

Edit `appsettings.json` to customize:
- ML model path
- Minimum confidence score thresholds
- Supported file formats
- Skill and soft skill databases

## Key Classes

### MLPredictionService
Handles all machine learning predictions using ML.NET framework:
- Skill detection and classification
- Certification identification
- Model training and storage

### ResumeAnalysisService
Core analysis engine that:
- Extracts skills, education, and experience
- Calculates category and overall scores
- Generates personalized recommendations
- Performs job requirement matching

### TextExtractionService
Extracts text from various formats:
- PDF files using iTextSharp
- DOCX files using OpenXML
- Plain text files

### ResumeStorageService
Manages data persistence:
- File system storage
- In-memory caching
- Search functionality

## API Methods

### ResumeAnalysisService

```csharp
// Analyze resume
Task<ResumeAnalysisResult> AnalyzeResumeAsync(Resume resume)

// Analyze against job description
Task<ResumeAnalysisResult> AnalyzeResumeAgainstJobAsync(
    Resume resume, string jobDescription)

// Calculate ATS score
Task<double> CalculateAtsScoreAsync(Resume resume)

// Generate recommendations
Task<List<string>> GenerateRecommendationsAsync(
    ResumeAnalysisResult analysis)
```

## ML.NET Features Used

- **Text Featurization**: Converts text to features for ML
- **Normalization**: Scales features for better predictions
- **SDCA Regression**: Trains models for skill scoring
- **Model Persistence**: Saves/loads trained models

## Output Format

Analysis results include:
- Overall Score (0-100)
- Extracted Skills with confidence scores
- Education and work experience
- Certifications
- Category-specific scores
- ATS compatibility assessment
- Personalized recommendations

## Error Handling

The application includes comprehensive error handling:
- File not found exceptions
- Text extraction errors
- ML model failures
- Storage operation errors

All errors are logged using Serilog for debugging and monitoring.

## Future Enhancements

- Advanced NLP models for better skill extraction
- Integration with job boards APIs
- Batch resume processing
- Resume improvement suggestions
- Interview question generation
- Salary prediction based on skills

## License

This project is created for educational and professional purposes.

## Support

For issues or questions, refer to the application logs in the console output.
