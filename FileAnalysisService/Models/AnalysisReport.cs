namespace FileAnalysisService.Models;

/// <summary>
/// Результат проверки файла на плагиат.
/// </summary>
public class AnalysisReport
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public double SimilarityScore { get; set; }
    public bool IsPlagiarism { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? WordCloudUrl { get; set; }
}

