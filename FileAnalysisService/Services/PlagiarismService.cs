using FileAnalysisService.Models;
using FileAnalysisService.Services.Abstractions;

namespace FileAnalysisService.Services;

/// <summary>
/// Сервис для имитации анализа файлов на плагиат и генерации отчетов.
/// </summary>
public class PlagiarismService : IPlagiarismService
{
    private readonly Dictionary<Guid, AnalysisReport> _reports = new();
    private readonly Random _random = new();

    /// <summary>
    /// Выполняет анализ файла: рассчитывает случайный процент схожести и генерирует облако слов.
    /// </summary>
    /// <param name="fileId">Идентификатор файла, который нужно проверить.</param>
    /// <returns>Сформированный отчет с результатами анализа.</returns>
    public Task<AnalysisReport> AnalyzeFileAsync(Guid fileId)
    {
        var similarity = _random.NextDouble();

        // Формируем URL для генерации облака слов через внешний API QuickChart.
        // Используем заглушку текста, так как физическое чтение файла в этом сервисе не реализовал.
        string dummyText = "microservices architecture docker csharp programming api gateway pattern swagger solid analysis";
        string chartUrl = $"https://quickchart.io/wordcloud?text={dummyText}&scale=linear&fontScale=50";

        var report = new AnalysisReport
        {
            Id = Guid.NewGuid(),
            FileId = fileId,
            SimilarityScore = Math.Round(similarity * 100, 2),
            IsPlagiarism = similarity > 0.8,
            CreatedAt = DateTime.UtcNow,
            WordCloudUrl = chartUrl
        };

        _reports[fileId] = report;

        return Task.FromResult(report);
    }

    /// <summary>
    /// Возвращает ранее созданный отчет по ID файла.
    /// </summary>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <returns>Отчет или null, если анализ еще не проводился.</returns>
    public Task<AnalysisReport?> GetReportByFileIdAsync(Guid fileId)
    {
        _reports.TryGetValue(fileId, out var report);
        return Task.FromResult(report);
    }
}
