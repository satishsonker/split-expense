using Microsoft.Extensions.Options;
using SplitExpense.Logger;
using SplitExpense.Models.ConfigModels;

namespace SplitExpense.FileManagement.Storage
{
    public class LocalStorageProvider(IOptions<FileUploadSettings> settings, ISplitExpenseLogger logger) : IStorageProvider
    {
        private readonly StorageSettings _settings = settings.Value.StorageSettings;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                var uploadPath = Path.Combine(_settings.BasePath);
                EnsureDirectoryExists(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using var fileStream2 = new FileStream(filePath, FileMode.Create);
                await fileStream.CopyToAsync(fileStream2);

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving file {fileName}");
                throw;
            }
        }

        public async Task<string?> SaveThumbnailAsync(Stream thumbnailStream, string fileName)
        {
            try
            {
                var thumbnailPath = Path.Combine(_settings.BasePath, _settings.ThumbnailPath);
                EnsureDirectoryExists(thumbnailPath);

                var filePath = Path.Combine(thumbnailPath, fileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await thumbnailStream.CopyToAsync(fileStream);

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving thumbnail {fileName}");
                return null;
            }
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file {filePath}");
                throw;
            }
        }

        public Task<bool> DeleteThumbnailAsync(string thumbnailPath)
        {
            return DeleteFileAsync(thumbnailPath);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
} 