namespace ProjectAero96.Helpers
{
    public interface IMailHelper
    {
        Task SendEmailAsync(string to, string subject, string body, params FileData[] attachments);
        Task SendEmailAsync(string to, string subject, string body, IEnumerable<FileData> attachments);
    }
}