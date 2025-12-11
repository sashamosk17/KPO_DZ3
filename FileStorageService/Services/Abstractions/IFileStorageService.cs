using FileStorageService.Models;

namespace FileStorageService.Services.Abstractions;

/// <summary>
/// Интерфейс для работы с хранилищем файлов
/// </summary>
public interface IFileStorageService
{
    Task<StoredFile> SaveFileAsync(IFormFile file);
    Task<(byte[] Content, string FileName)?> GetFileAsync(Guid fileId);
    Task<bool> DeleteFileAsync(Guid fileId);
}
