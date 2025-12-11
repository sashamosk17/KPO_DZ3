namespace FileStorageService.Models;

/// <summary>
/// Представляет информацию о сохранённом файле
/// </summary>
public class StoredFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public long FileSize { get; set; }
}
