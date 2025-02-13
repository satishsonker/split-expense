using SplitExpense.Models.DTO;

namespace SplitExpense.FileManagement.Storage
{
    public interface IStorageProvider
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType);
        Task<string?> SaveThumbnailAsync(Stream thumbnailStream, string fileName);
        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> DeleteThumbnailAsync(string thumbnailPath);
    }
} 