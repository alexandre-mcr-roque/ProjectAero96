using ProjectAero96.Models;

namespace ProjectAero96.Helpers
{
    public interface IMailHelper
    {
        Task SendEmailAsync(string to, string subject, string body, params IMailFileModel[] attachments);
    }
}