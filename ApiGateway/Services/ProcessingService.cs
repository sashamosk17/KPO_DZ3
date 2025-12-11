using ApiGateway.Services.Abstractions;
using System.Net.Http.Json;

namespace ApiGateway.Services;

/// <summary>
/// Сервис, управляющий процессом загрузки и проверки файлов.
/// </summary>
public class ProcessingService : IProcessingService
{
    private readonly HttpClient _httpClient;
    private readonly string _storageUrl;
    private readonly string _analysisUrl;

    public ProcessingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _storageUrl = configuration["StorageServiceUrl"] ?? "https://localhost:7056";
        _analysisUrl = configuration["AnalysisServiceUrl"] ?? "https://localhost:7186";
    }

    /// <summary>
    /// Загружает файл в сервис хранения (FileStorageService).
    /// </summary>
    /// <param name="file">Файл, полученный от клиента.</param>
    /// <returns>Уникальный идентификатор сохраненного файла.</returns>
    /// <exception cref="Exception">Выбрасывается, если сервис хранения недоступен.</exception>
    public async Task<Guid> UploadFileAsync(IFormFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"{_storageUrl}/api/files", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UploadResult>();
            return result?.FileId ?? Guid.Empty;
        }
        catch (HttpRequestException)
        {
            // Обрабатываем падение микросервиса, чтобы вернуть понятную ошибку
            throw new Exception("Сервис хранения файлов (FileStorageService) недоступен. Попробуйте позже :(.");
        }
    }

    /// <summary>
    /// Инициирует проверку файла на плагиат в сервисе анализа (FileAnalysisService).
    /// </summary>
    /// <param name="fileId">Идентификатор файла, который нужно проверить.</param>
    /// <returns>Объект отчета с результатами анализа.</returns>
    /// <exception cref="Exception">Выбрасывается, если сервис анализа недоступен.</exception>
    public async Task<object> CheckForPlagiarismAsync(Guid fileId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"{_analysisUrl}/api/reports/{fileId}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>();
        }
        catch (HttpRequestException)
        {
            throw new Exception("Сервис анализа (FileAnalysisService) недоступен. Попробуйте позже. :(");
        }
    }
}

public class UploadResult { public Guid FileId { get; set; } }
