using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.FileManagement.Storage;
using SplitExpense.Logger;
using SplitExpense.Models.ConfigModels;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.FileManagement.Service
{
    public class FileUploadService(
        IStorageProvider storageProvider,
        IOptions<FileUploadSettings> settings,
        ISplitExpenseLogger logger) : IFileUploadService
    {
        private readonly IStorageProvider _storageProvider = storageProvider;
        private readonly FileUploadSettings _settings = settings.Value;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<List<FileUploadResponse>> UploadFilesAsync(IFormFileCollection files)
        {
            var responses = new List<FileUploadResponse>();
            foreach (var file in files)
            {
                try
                {
                    var response = await UploadFileAsync(file);
                    responses.Add(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error uploading file {file.FileName}");
                    throw;
                }
            }
            return responses;
        }

        public async Task<FileUploadResponse> UploadFileAsync(IFormFile file)
        {
            try
            {
                ValidateFile(file);

                var fileType = GetFileType(file);
                var uniqueFileName = GenerateUniqueFileName(file.FileName);
                string? thumbnailPath = null;

                using var stream = file.OpenReadStream();
                var filePath = await _storageProvider.SaveFileAsync(stream, uniqueFileName, file.ContentType);

                if (fileType == FileType.Image)
                {
                    thumbnailPath = await CreateAndSaveThumbnailAsync(file, uniqueFileName);
                }

                return new FileUploadResponse
                {
                    FileName = uniqueFileName,
                    OriginalFileName = file.FileName,
                    FilePath = filePath,
                    ThumbnailPath = thumbnailPath,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    FileExtension = Path.GetExtension(file.FileName).ToLower(),
                    FileType = fileType,
                    UploadedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file {file.FileName}");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath, string? thumbnailPath = null)
        {
            try
            {
                var result = await _storageProvider.DeleteFileAsync(filePath);
                if (thumbnailPath != null)
                {
                    result &= await _storageProvider.DeleteThumbnailAsync(thumbnailPath);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file {filePath}");
                throw;
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BusinessRuleViolationException(ErrorCodes.InvalidFile.GetDescription(), "File is empty");

            var extension = Path.GetExtension(file.FileName).ToLower();
            var fileType = GetFileType(file);

            // Validate file extension
            var allowedExtensions = fileType == FileType.Image 
                ? _settings.AllowedImageExtensions 
                : _settings.AllowedDocumentExtensions;

            if (!allowedExtensions.Contains(extension))
                throw new BusinessRuleViolationException(ErrorCodes.InvalidFileExtension.GetDescription(), 
                    $"File extension {extension} is not allowed");

            // Get size limit based on file type and extension
            long maxSizeInMB = GetMaxFileSizeInMB(extension, fileType);
            long maxSizeInBytes = maxSizeInMB * 1024 * 1024;

            if (file.Length > maxSizeInBytes)
                throw new BusinessRuleViolationException(ErrorCodes.FileSizeExceeded.GetDescription(), 
                    $"File size ({FormatFileSize(file.Length)}) exceeds maximum limit of {maxSizeInMB}MB");
        }

        private int GetMaxFileSizeInMB(string extension, FileType fileType)
        {
            // Check for extension-specific size limit first
            if (_settings.FileSizeSettings.ExtensionSpecificSizeLimits.TryGetValue(extension, out int specificLimit))
            {
                return specificLimit;
            }

            // If no specific limit, use default based on file type
            return fileType == FileType.Image 
                ? _settings.FileSizeSettings.MaxImageSizeInMB 
                : _settings.FileSizeSettings.MaxDocumentSizeInMB;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        private FileType GetFileType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            return _settings.AllowedImageExtensions.Contains(extension) 
                ? FileType.Image 
                : FileType.Document;
        }

        private async Task<string?> CreateAndSaveThumbnailAsync(IFormFile file, string uniqueFileName)
        {
            try
            {
                using var image = await Image.LoadAsync(file.OpenReadStream());
                var thumbFileName = $"thumb_{uniqueFileName}";

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(_settings.ThumbnailSettings.Width, _settings.ThumbnailSettings.Height),
                    Mode = _settings.ThumbnailSettings.MaintainAspectRatio 
                        ? ResizeMode.Max 
                        : ResizeMode.Stretch
                }));

                using var memoryStream = new MemoryStream();
                await image.SaveAsync(memoryStream, image.Metadata.DecodedImageFormat);
                memoryStream.Position = 0;

                return await _storageProvider.SaveThumbnailAsync(memoryStream, thumbFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating thumbnail for {file.FileName}");
                return null;
            }
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{extension}";
            return uniqueName;
        }
    }
} 