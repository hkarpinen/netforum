namespace NETForum.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string? contentType = null);
    Task<Stream> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExists(string filePath);
    string GetFileUrl(string filePath);
}