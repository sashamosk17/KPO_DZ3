using FileStorageService.Models;
using FileStorageService.Services.Abstractions;

namespace FileStorageService.Services;

/// <summary>
/// Сервис для сохранения и получения файлов с диска.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly Dictionary<Guid, StoredFile> _fileMetadata = new();

    public FileStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["StoragePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Storage");

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    /// <summary>
    /// Асинхронно сохраняет файл на диск, присваивая ему уникальный идентификатор.
    /// </summary>
    /// <param name="file">Загружаемый файл.</param>
    /// <returns>Метаданные сохраненного файла.</returns>
    public async Task<StoredFile> SaveFileAsync(IFormFile file)
    {
        var fileId = Guid.NewGuid();
        var filePath = Path.Combine(_storagePath, fileId.ToString());

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var storedFile = new StoredFile
        {
            Id = fileId,
            FileName = file.FileName,
            FilePath = filePath,
            UploadedAt = DateTime.UtcNow,
            FileSize = file.Length
        };

        _fileMetadata[fileId] = storedFile;
        return storedFile;
    }


    /// <summary>
    /// Получает файл с диска по идентификатору.
    /// </summary>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <returns>Кортеж с содержимым файла и именем, или null, если не найден.</returns>
    public async Task<(byte[] Content, string FileName)?> GetFileAsync(Guid fileId)
    {
        if (!_fileMetadata.TryGetValue(fileId, out var metadata))
            return null;

        if (!File.Exists(metadata.FilePath))
            return null;

        var content = await File.ReadAllBytesAsync(metadata.FilePath);
        return (content, metadata.FileName);
    }





    /// <summary>
    /// Удаляет файл с диска и из памяти.
    /// </summary>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <returns>Результат удаления.</returns>
    public Task<bool> DeleteFileAsync(Guid fileId)
    {
        if (!_fileMetadata.TryGetValue(fileId, out var metadata))
            return Task.FromResult(false);

        if (File.Exists(metadata.FilePath))
        {
            File.Delete(metadata.FilePath);
        }

        _fileMetadata.Remove(fileId);
        return Task.FromResult(true);
    }
}
