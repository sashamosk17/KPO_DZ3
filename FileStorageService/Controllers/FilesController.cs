using FileStorageService.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Загрузить файл в хранилище
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не был предоставлен");

        var storedFile = await _fileStorageService.SaveFileAsync(file);
        return Ok(new { fileId = storedFile.Id, fileName = storedFile.FileName });
    }

    /// <summary>
    /// Получить файл по ID
    /// </summary>
    [HttpGet("{fileId:guid}")]
    public async Task<IActionResult> Download(Guid fileId)
    {
        var result = await _fileStorageService.GetFileAsync(fileId);

        if (result == null)
            return NotFound();

        return File(result.Value.Content, "application/octet-stream", result.Value.FileName);
    }

    /// <summary>
    /// Удалить файл
    /// </summary>
    [HttpDelete("{fileId:guid}")]
    public async Task<IActionResult> Delete(Guid fileId)
    {
        var success = await _fileStorageService.DeleteFileAsync(fileId);
        return success ? Ok() : NotFound();
    }
}
