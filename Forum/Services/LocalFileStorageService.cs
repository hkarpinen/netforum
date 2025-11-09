namespace NETForum.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public LocalFileStorageService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        _storagePath = Path.Combine(
            _webHostEnvironment.WebRootPath, 
            configuration["FileStorage:Local:StoragePath"] ?? "uploads");
        Directory.CreateDirectory(_storagePath);
    }
    
    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_storagePath, uniqueFileName);
        await using var fileStreamOut = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOut);
        return uniqueFileName;
    }

    public async Task<Stream> GetFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_storagePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        
        var memoryStream = new MemoryStream();
        await using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_storagePath, filePath);
        if (!File.Exists(fullPath)) return Task.FromResult(false);
        File.Delete(fullPath);
        return Task.FromResult(true);
    }

    public Task<bool> FileExists(string filePath)
    {
        var fullPath = Path.Combine(_storagePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }
    
    public string GetFileUrl(string filePath)
    {
        var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, _storagePath);
        var url = $"/{relativePath}/{filePath}".Replace("\\", "/");
        return url;
    }
}