using Azure.Storage.Blobs;

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

        public async Task<string?> UploadUserImageAsync(byte[] image, string? oldImageId)
            => await UploadImageAsync(image, "user-images", oldImageId);


        public async Task<string?> UploadUserImageAsync(string imageUrl, string? oldImageId)
            => await UploadImageAsync(imageUrl, "user-images", oldImageId);


        //========================================================================
        // Private methods for handling image uploads
        //========================================================================
        private async Task<string?> UploadImageAsync(IFormFile image, string containerName, string? oldImageId)
        {
            try
            {
                Stream stream = image.OpenReadStream();
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
                Stream stream = new MemoryStream(image);
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
                Stream stream = File.OpenRead(imageUrl);
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
            // Upload complete, dispose stream
            await stream.DisposeAsync();
            return name;
        }
    }
}
