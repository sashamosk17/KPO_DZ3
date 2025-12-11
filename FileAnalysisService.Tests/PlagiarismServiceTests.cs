using AutoFixture;
using FileAnalysisService.Models;
using FileAnalysisService.Services;
using Xunit;

namespace FileAnalysisService.Tests;

/// <summary>
/// Тесты для <see cref="PlagiarismService"/>.
/// Проверяет логику анализа файлов, генерацию отчетов и их сохранение в памяти.
/// </summary>
public class PlagiarismServiceTests
{
    private readonly Fixture _fixture;
    private readonly PlagiarismService _service;

    public PlagiarismServiceTests()
    {
        _fixture = new Fixture();
        _service = new PlagiarismService();
    }

    /// <summary>
    /// Проверяет, что метод анализа возвращает корректно заполненный отчет. Убеждается в генерации ID, расчете процентов схожести и формировании ссылки на облако слов.
    /// </summary>
    [Fact]
    public async Task AnalyzeFileAsync_ShouldReturnValidReport()
    {
        var fileId = _fixture.Create<Guid>();

        var result = await _service.AnalyzeFileAsync(fileId);

        Assert.NotNull(result);
        Assert.Equal(fileId, result.FileId);

        Assert.InRange(result.SimilarityScore, 0, 100);

        Assert.NotNull(result.WordCloudUrl);
        Assert.StartsWith("https://quickchart.io", result.WordCloudUrl);

        Assert.NotEqual(DateTime.MinValue, result.CreatedAt);
    }

    /// <summary>
    /// Проверяет, что отчет сохраняется в памяти сервиса после анализа
    /// и может быть успешно извлечен по айди файла.
    /// </summary>
    [Fact]
    public async Task GetReportByFileIdAsync_ShouldReturnReport_AfterAnalysis()
    {
        var fileId = _fixture.Create<Guid>();
        await _service.AnalyzeFileAsync(fileId);
        var report = await _service.GetReportByFileIdAsync(fileId);
        Assert.NotNull(report);
        Assert.Equal(fileId, report.FileId);
    }

    /// <summary>
    /// Проверяет, что метод возвращает null, если отчет для указанного айди не был найден.
    /// </summary>
    [Fact]
    public async Task GetReportByFileIdAsync_ShouldReturnNull_IfReportNotExists()
    {
        var randomId = _fixture.Create<Guid>();
        var report = await _service.GetReportByFileIdAsync(randomId);
        Assert.Null(report);
    }
}
