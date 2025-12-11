using FileAnalysisService.Models;

namespace FileAnalysisService.Services.Abstractions;

public interface IPlagiarismService
{
    Task<AnalysisReport> AnalyzeFileAsync(Guid fileId);
    Task<AnalysisReport?> GetReportByFileIdAsync(Guid fileId);
}
