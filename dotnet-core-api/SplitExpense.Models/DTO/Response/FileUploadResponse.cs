namespace SplitExpense.Models.DTO
{
    public class FileUploadResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? ThumbnailPath { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileExtension { get; set; } = string.Empty;
        public FileType FileType { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public enum FileType
    {
        Image,
        Document
    }
} 