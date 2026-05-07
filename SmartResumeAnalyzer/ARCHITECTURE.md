/*
 * ARCHITECTURE AND DESIGN PATTERNS
 * 
 * This document describes the architecture and design patterns used in Smart Resume Analyzer.
 */

# Architecture Overview

## Layered Architecture

The application follows a clean layered architecture:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│        (Console UI - Program.cs)        │
└────────────────────┬────────────────────┘
                     │
┌─────────────────────▼────────────────────┐
│         Application Layer               │
│  (ResumeAnalysisService, etc.)          │
└────────────────────┬────────────────────┘
                     │
┌─────────────────────▼────────────────────┐
│         Business Logic Layer            │
│ (MLPredictionService, TextExtraction)   │
└────────────────────┬────────────────────┘
                     │
┌─────────────────────▼────────────────────┐
│         Data Access Layer               │
│   (ResumeStorageService, Databases)     │
└─────────────────────────────────────────┘
```

## Design Patterns Used

### 1. Dependency Injection (DI)
- **Where**: Program.cs ServiceCollection setup
- **Purpose**: Loose coupling between services
- **Example**: 
  ```csharp
  services.AddScoped<IResumeAnalysisService, ResumeAnalysisService>();
  ```

### 2. Repository Pattern
- **Where**: ResumeStorageService
- **Purpose**: Abstract data access logic
- **Benefits**: Easy to swap implementations (file, database, cloud)

### 3. Strategy Pattern
- **Where**: Text extraction with different file formats
- **Purpose**: Encapsulate algorithm families
- **Example**: 
  ```csharp
  switch(extension) {
      case ".pdf": await ExtractFromPdfAsync();
      case ".docx": await ExtractFromDocxAsync();
  }
  ```

### 4. Factory Pattern
- **Where**: Service creation in DI container
- **Purpose**: Centralized object creation
- **Benefits**: Consistent initialization and configuration

### 5. Builder Pattern
- **Where**: Complex object construction (Resume, Analysis)
- **Purpose**: Construct objects step by step
- **Example**: ResumeAnalysisResult builds incrementally

### 6. Observer Pattern
- **Where**: Logging throughout the application
- **Purpose**: Decouple logging from business logic
- **Tool**: Serilog

## Data Flow

```
User Input (File)
    ↓
TextExtractionService (Extract Text)
    ↓
Resume Object (Content populated)
    ↓
MLPredictionService (Analyze Content)
    ↓
ResumeAnalysisService (Process Results)
    ↓
ResumeAnalysisResult (Analysis Complete)
    ↓
ResumeStorageService (Persist Data)
    ↓
Display Results to User
```

## Service Responsibilities

### MLPredictionService
- Train ML models
- Make predictions
- Save/Load models
- Skill and certification detection

### ResumeAnalysisService
- Orchestrate analysis
- Extract information
- Calculate scores
- Generate recommendations

### TextExtractionService
- Handle different file formats
- Extract readable text
- Validate extracted content

### ResumeStorageService
- Save resume data
- Retrieve resume data
- Search functionality
- In-memory caching

## Configuration Management

Configuration is handled through `appsettings.json`:
```json
{
  "ResumeAnalyzer": {
    "ModelPath": "./Models/trained_model.zip",
    "MinConfidenceScore": 0.65,
    "MaxResumeLength": 10000
  }
}
```

Loaded via `IConfiguration` interface for flexibility and testability.

## Async/Await Usage

Most I/O operations are asynchronous:
- File operations: `File.ReadAllTextAsync()`
- ML operations: `PredictSkillsAsync()`
- Storage operations: `SaveResumeAsync()`

Benefits:
- Non-blocking operations
- Better scalability
- Responsive UI

## Error Handling Strategy

```
try {
    // Operation
} catch (SpecifcException ex) {
    _logger.LogError(ex, "Specific context");
    // Handle or rethrow
} catch (Exception ex) {
    _logger.LogError(ex, "Generic error");
    // General handling
}
```

All exceptions are logged for debugging.

## Testing Strategy

### Unit Testing (Recommended)
- Test individual services in isolation
- Mock dependencies using Moq
- Test data models
- Test utility functions

Example:
```csharp
[Test]
public async Task AnalyzeResume_ValidResume_ReturnsAnalysis()
{
    // Arrange
    var service = new ResumeAnalysisService(...);
    var resume = new Resume { Content = "..." };
    
    // Act
    var result = await service.AnalyzeResumeAsync(resume);
    
    // Assert
    Assert.IsNotNull(result);
    Assert.Greater(result.OverallScore, 0);
}
```

### Integration Testing
- Test service interactions
- Use test data
- Verify end-to-end workflows

## Extensibility Points

### Adding New Skills
Edit `SkillDatabase.cs` to add new categories and skills.

### Adding New File Formats
Implement new extraction method in `TextExtractionService`.

### Customizing Scoring
Override scoring logic in `ResumeAnalysisService.CalculateCategoryScores()`.

### Training New Models
Use `MLPredictionService.TrainSkillsModelAsync()` with your data.

## Performance Considerations

1. **Caching**: Resumes cached in memory after loading
2. **Async Operations**: Non-blocking I/O throughout
3. **Model Optimization**: ML.NET models optimized for prediction speed
4. **Text Processing**: Efficient string operations in utilities

## Security Best Practices

1. **Input Validation**: Check file types and content
2. **Error Messages**: Don't expose sensitive information
3. **Logging**: Secure logging without sensitive data
4. **Configuration**: Sensitive config in user secrets (for production)

## Deployment Considerations

### Console Application
- Single executable
- Self-contained deployment ready
- Easy to containerize with Docker

### As a Library
- Expose services through public APIs
- Package as NuGet package
- Reference in other .NET applications

### As a Web Service
- Wrap services with ASP.NET Core
- Create REST/GraphQL endpoints
- Host on Azure, AWS, or on-premises
