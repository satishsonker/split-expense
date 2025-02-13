
namespace SplitExpense.FileManagement.Storage
{
    public class AzureBlobStorageProvider : IStorageProvider
    {
        public Task<bool> DeleteFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteThumbnailAsync(string thumbnailPath)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<string?> SaveThumbnailAsync(Stream thumbnailStream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
