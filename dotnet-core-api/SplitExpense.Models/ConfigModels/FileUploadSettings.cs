namespace SplitExpense.Models.ConfigModels
{
    public class FileUploadSettings
    {
        public FileSizeSettings FileSizeSettings { get; set; } = new();
        public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif"];
        public string[] AllowedDocumentExtensions { get; set; } = [".pdf", ".doc", ".docx", ".xls", ".xlsx"];
        public ThumbnailSettings ThumbnailSettings { get; set; } = new();
        public StorageSettings StorageSettings { get; set; } = new();
    }

    public class FileSizeSettings
    {
        public int MaxImageSizeInMB { get; set; } = 5;
        public int MaxDocumentSizeInMB { get; set; } = 10;
        public Dictionary<string, int> ExtensionSpecificSizeLimits { get; set; } = new()
        {
            { ".pdf", 15 },    // 15MB for PDFs
            { ".psd", 20 }     // 20MB for Photoshop files
        };
    }

    public class ThumbnailSettings
    {
        public int Width { get; set; } = 150;
        public int Height { get; set; } = 150;
        public bool MaintainAspectRatio { get; set; } = true;
    }

    public class StorageSettings
    {
        public StorageType StorageType { get; set; } = StorageType.Local;
        public string BasePath { get; set; } = "uploads";
        public string ThumbnailPath { get; set; } = "thumbnails";
        public AzureBlobSettings AzureBlob { get; set; } = new();
        public AwsS3Settings AwsS3 { get; set; } = new();
        public ApiSettings Api { get; set; } = new();
    }

    public class AzureBlobSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
    }

    public class AwsS3Settings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }

    public class ApiSettings
    {
        public string Url { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public enum StorageType
    {
        Local,
        AzureBlob,
        AwsS3,
        Api
    }
} 