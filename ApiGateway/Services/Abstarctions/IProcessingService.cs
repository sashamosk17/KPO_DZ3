namespace ApiGateway.Services.Abstractions;

public interface IProcessingService
{
    // Отправить файл и получить ID
    Task<Guid> UploadFileAsync(IFormFile file);

    // Запустить проверку
    Task<object> CheckForPlagiarismAsync(Guid fileId);
}
