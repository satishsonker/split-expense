using Microsoft.AspNetCore.Http;
using SplitExpense.Models.DTO;

namespace SplitExpense.FileManagement.Service
{
    public interface IFileUploadService
    {
        Task<List<FileUploadResponse>> UploadFilesAsync(IFormFileCollection files);
        Task<FileUploadResponse> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string filePath, string? thumbnailPath = null);
    }
} 