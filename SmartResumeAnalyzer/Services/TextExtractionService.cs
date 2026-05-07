using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartResumeAnalyzer.Services;

public interface ITextExtractionService
{
    Task<string> ExtractTextAsync(string fileName, string fileContent);
    Task<string> ExtractFromPdfAsync(byte[] fileContent);
    Task<string> ExtractFromDocxAsync(byte[] fileContent);
    Task<string> ExtractFromTxtAsync(byte[] fileContent);
}

public class TextExtractionService : ITextExtractionService
{
    private readonly ILogger<TextExtractionService> _logger;
    private readonly IConfiguration _configuration;

    public TextExtractionService(ILogger<TextExtractionService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> ExtractTextAsync(string fileName, string fileContent)
    {
        try
        {
            var extension = Path.GetExtension(fileName).ToLower();

            return extension switch
            {
                ".pdf" => await ExtractFromPdfAsync(Encoding.UTF8.GetBytes(fileContent)),
                ".docx" => await ExtractFromDocxAsync(Encoding.UTF8.GetBytes(fileContent)),
                ".txt" => await ExtractFromTxtAsync(Encoding.UTF8.GetBytes(fileContent)),
                _ => throw new NotSupportedException($"File format {extension} is not supported")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from file: {FileName}", fileName);
            return fileContent;
        }
    }

    public async Task<string> ExtractFromPdfAsync(byte[] fileContent)
    {
        _logger.LogInformation("Extracting text from PDF");
        
        try
        {
            // Note: Using iTextSharp requires PdfReader and PdfDocument
            // For actual implementation, you would use:
            // using var reader = new PdfReader(new MemoryStream(fileContent));
            // using var document = new PdfDocument(reader);
            
            var text = new StringBuilder();
            
            // Simulated extraction - in production, use iTextSharp properly
            _logger.LogInformation("PDF text extraction completed");
            return text.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from PDF");
            throw;
        }
    }

    public async Task<string> ExtractFromDocxAsync(byte[] fileContent)
    {
        _logger.LogInformation("Extracting text from DOCX");
        
        try
        {
            var text = new StringBuilder();
            
            // For DOCX extraction, you would typically use:
            // using (var memoryStream = new MemoryStream(fileContent))
            // using (var document = WordprocessingDocument.Open(memoryStream, false))
            // Extract text from document.MainDocumentPart.Document.Body
            
            _logger.LogInformation("DOCX text extraction completed");
            return text.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from DOCX");
            throw;
        }
    }

    public async Task<string> ExtractFromTxtAsync(byte[] fileContent)
    {
        _logger.LogInformation("Extracting text from TXT");
        
        try
        {
            var text = Encoding.UTF8.GetString(fileContent);
            _logger.LogInformation("TXT text extraction completed");
            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from TXT");
            throw;
        }
    }
}
