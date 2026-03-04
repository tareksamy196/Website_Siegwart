using Microsoft.AspNetCore.Http;

namespace Website.Siegwart.BLL.Services.Interfaces
{
    /// <summary>
    /// Service for handling file uploads and deletions
    /// </summary>
    public interface IAttachmentService
    {
        /// <summary>
        /// Upload a file to the specified folder
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="folderPath">Relative folder path (e.g., "uploads/products")</param>
        /// <returns>Relative path of the uploaded file, or null if failed</returns>
        Task<string?> UploadAsync(IFormFile file, string folderPath);

        /// <summary>
        /// Delete a file by its relative path
        /// </summary>
        /// <param name="fileRelativePath">Relative path of the file to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        bool Delete(string fileRelativePath);
    }
}