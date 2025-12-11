using FileAnalysisService.Models;
using FileAnalysisService.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IPlagiarismService _plagiarismService;

    public ReportsController(IPlagiarismService plagiarismService)
    {
        _plagiarismService = plagiarismService;
    }

    /// <summary>
    /// Запустить проверку файла на плагиат
    /// </summary>
    [HttpPost("{fileId:guid}")]
    public async Task<IActionResult> Analyze(Guid fileId)
    {
        var report = await _plagiarismService.AnalyzeFileAsync(fileId);
        return Ok(report);
    }

    /// <summary>
    /// Получить отчет по ID файла
    /// </summary>
    [HttpGet("{fileId:guid}")]
    public async Task<IActionResult> GetReport(Guid fileId)
    {
        var report = await _plagiarismService.GetReportByFileIdAsync(fileId);

        if (report == null)
            return NotFound("Отчет для данного файла не найден");

        return Ok(report);
    }
}
