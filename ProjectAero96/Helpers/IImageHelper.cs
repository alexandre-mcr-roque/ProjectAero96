namespace ProjectAero96.Helpers
{
    public interface IImageHelper
    {
        Task<string?> UploadUserImageAsync(IFormFile image, string? oldImageId = null);
        Task<string?> UploadUserImageAsync(byte[] image, string? oldImageId = null);
        Task<string?> UploadUserImageAsync(string imageUrl, string? oldImageId = null);
        Task DeleteUserImageAsync(string imageId);

        Task<string?> UploadAirlineImageAsync(IFormFile image, string? oldImageId = null);
        Task<string?> UploadAirlineImageAsync(byte[] image, string? oldImageId = null);
        Task<string?> UploadAirlineImageAsync(string imageUrl, string? oldImageId = null);
        Task DeleteAirlineImageAsync(string imageId);
    }
}
