using Azure.Storage.Blobs;
using System.IO;

namespace ProjectAero96.Helpers
{
    public class ImageHelper : IImageHelper
    {
        private readonly BlobServiceClient blobService;

        public ImageHelper(IConfiguration configuration)
        {
            string keys = configuration["ConnectionStrings:Blobs"]!;
            blobService = new BlobServiceClient(keys);
        }

        public async Task<string?> UploadUserImageAsync(IFormFile image, string? oldImageId = null)
            => await UploadImageAsync(image, "user-images", oldImageId);
        public async Task<string?> UploadUserImageAsync(byte[] image, string? oldImageId = null)
            => await UploadImageAsync(image, "user-images", oldImageId);
        public async Task<string?> UploadUserImageAsync(string imageUrl, string? oldImageId = null)
            => await UploadImageAsync(imageUrl, "user-images", oldImageId);
        public async Task DeleteUserImageAsync(string imageId)
            => await DeleteImageAsync(imageId, "user-images");

        public async Task<string?> UploadAirlineImageAsync(IFormFile image, string? oldImageId = null)
            => await UploadImageAsync(image, "airline-images", oldImageId);
        public async Task<string?> UploadAirlineImageAsync(byte[] image, string? oldImageId = null)
            => await UploadImageAsync(image, "airline-images", oldImageId);
        public async Task<string?> UploadAirlineImageAsync(string imageUrl, string? oldImageId = null)
            => await UploadImageAsync(imageUrl, "airline-images", oldImageId);
        public async Task DeleteAirlineImageAsync(string imageId)
            => await DeleteImageAsync(imageId, "airline-images");



        //========================================================================
        // Private methods for handling image uploads
        //========================================================================
        private async Task<string?> UploadImageAsync(IFormFile image, string containerName, string? oldImageId)
        {
            try
            {
                using Stream stream = image.OpenReadStream();
                return await UploadStreamAsync(stream, containerName, oldImageId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading image: {ex.Message}");
                return null;
            }
        }

        private async Task<string?> UploadImageAsync(byte[] image, string containerName, string? oldImageId)
        {
            try
            {
                using Stream stream = new MemoryStream(image);
                return await UploadStreamAsync(stream, containerName, oldImageId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading image: {ex.Message}");
                return null;
            }
        }

        private async Task<string?> UploadImageAsync(string imageUrl, string containerName, string? oldImageId)
        {
            try
            {
                using Stream stream = File.OpenRead(imageUrl);
                return await UploadStreamAsync(stream, containerName, oldImageId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading image: {ex.Message}");
                return null;
            }
        }

        private async Task<string> UploadStreamAsync(Stream stream, string containerName, string? oldImageId)
        {
            string name = oldImageId != null ? oldImageId : Guid.NewGuid().ToString();
            var containerClient = blobService.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name.ToString());
            await blobClient.UploadAsync(stream, overwrite: true);
            return name;
        }
        private async Task DeleteImageAsync(string imageId, string containerName)
        {
            try
            {
                var containerClient = blobService.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(imageId);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting image: {ex.Message}");
            }
        }
    }
}
