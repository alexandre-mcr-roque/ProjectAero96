namespace ProjectAero96.Helpers
{
    public interface IImageHelper
    {
        public Task<string?> UploadUserImageAsync(IFormFile image, string? oldImageId = null);
        public Task<string?> UploadUserImageAsync(byte[] image, string? oldImageId = null);
        public Task<string?> UploadUserImageAsync(string imageUrl, string? oldImageId = null);
    }
}
