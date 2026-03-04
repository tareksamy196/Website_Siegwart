using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Website.Siegwart.BLL.Services.Classes
{
    public class AttachmentService : IAttachmentService
    {
        // Allowed file extensions
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".pdf", ".doc", ".docx"
        };

        // Max file size: 5 MB
        private const int MaxFileSize = 5 * 1024 * 1024;

        private readonly string _rootPath;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(IWebHostEnvironment webHostEnvironment, ILogger<AttachmentService> logger)
        {
            _rootPath = webHostEnvironment.WebRootPath ?? throw new ArgumentNullException(nameof(webHostEnvironment.WebRootPath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string?> UploadAsync(IFormFile file, string folderPath)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload attempt with null or empty file");
                    throw new ArgumentException("No file uploaded.");
                }

                // Check extension
                var extension = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                {
                    _logger.LogWarning("Invalid file type attempted: {Extension}", extension);
                    throw new InvalidOperationException($"Invalid file type: {extension}");
                }

                // Check size
                if (file.Length > MaxFileSize)
                {
                    _logger.LogWarning("File size exceeds limit: {Size} bytes", file.Length);
                    throw new InvalidOperationException("File size exceeds the maximum limit (5MB).");
                }

                // Validate image content (Magic Bytes) for security
                if (IsImageFile(extension) && !IsValidImage(file))
                {
                    _logger.LogWarning("Invalid image file attempted");
                    throw new InvalidOperationException("Invalid image file.");
                }

                // Prepare directory
                string directory = Path.Combine(_rootPath, folderPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Generate unique filename
                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(directory, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path
                var relativePath = Path.Combine(folderPath, uniqueFileName).Replace("\\", "/");
                _logger.LogInformation("File uploaded successfully: {FileName} to {Path}", uniqueFileName, relativePath);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
                throw;
            }
        }

        public bool Delete(string fileRelativePath)
        {
            try
            {
                // Validate parameter
                if (string.IsNullOrWhiteSpace(fileRelativePath))
                {
                    _logger.LogWarning("Delete attempt with null or empty path");
                    return false;
                }

                string fullPath = Path.Combine(_rootPath, fileRelativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                // Check if file exists and delete
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted successfully: {Path}", fileRelativePath);
                    return true;
                }

                _logger.LogWarning("File not found for deletion: {Path}", fileRelativePath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {Path}", fileRelativePath);
                return false;
            }
        }

        // Helper: Check if file extension is for images
        private static bool IsImageFile(string extension)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            return imageExtensions.Contains(extension.ToLowerInvariant());
        }

        // Helper: Validate image by checking Magic Bytes (security)
        private bool IsValidImage(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var buffer = new byte[8];
                stream.Read(buffer, 0, 8);

                // PNG: 89 50 4E 47 0D 0A 1A 0A
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                    return true;

                // JPEG: FF D8 FF
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                    return true;

                // WebP: RIFF ... WEBP
                if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46)
                {
                    stream.Position = 8;
                    var webpBuffer = new byte[4];
                    stream.Read(webpBuffer, 0, 4);
                    if (webpBuffer[0] == 0x57 && webpBuffer[1] == 0x45 && webpBuffer[2] == 0x42 && webpBuffer[3] == 0x50)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating image file");
                return false;
            }
        }
    }
}