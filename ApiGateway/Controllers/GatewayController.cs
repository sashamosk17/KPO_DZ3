using ApiGateway.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    private readonly IProcessingService _processingService;

    public GatewayController(IProcessingService processingService)
    {
        _processingService = processingService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitWork(IFormFile file)
    {
        try
        {
            var fileId = await _processingService.UploadFileAsync(file);
            var report = await _processingService.CheckForPlagiarismAsync(fileId);

            return Ok(new
            {
                Message = "Работа успешно загружена и проверена",
                FileId = fileId,
                Report = report
            });
        }
        catch (Exception ex)
        {
            // Возвращаем 503 (Service Unavailable) с текстом ошибки
            return StatusCode(503, new { Error = ex.Message });
        }
    }

}
