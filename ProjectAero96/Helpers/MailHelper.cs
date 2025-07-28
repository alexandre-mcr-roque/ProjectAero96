using MailKit.Net.Smtp;
using MimeKit;

namespace ProjectAero96.Helpers
{
    public class MailHelper : IMailHelper, IDisposable
    {
        private readonly SmtpClient smtpClient;
        private readonly MailboxAddress fromAddress;
        private bool _disposed;

        public MailHelper(IConfiguration configuration)
        {
            string name = configuration["Mail:Name"]!;
            string address = configuration["Mail:Address"]!;
            string smtp = configuration["Mail:Smtp"]!;
            string port = configuration["Mail:Port"]!;
            string password = configuration["Mail:Password"]!;

            smtpClient = new SmtpClient();
            smtpClient.Connect(smtp, int.Parse(port));
            smtpClient.Authenticate(address, password);

            fromAddress = new MailboxAddress(name, address);
        }
        public async Task SendEmailAsync(string to, string subject, string body, params FileData[] attachments)
        {
            var message = new MimeMessage();
            message.From.Add(fromAddress);
            message.To.Add(new MailboxAddress(to,to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = $"""
                <center>
                    <div style="font-family:Arial,Helvetica,sans-serif;background-color:#efefef;width:fit-content;padding:30px 30px 10px;color:#000">
                        <div style="font-size:1.2rem">
                            {body}
                        </div>
                        <br><br>
                        <span style="font-size:0.7rem;color:#636363">This email was sent automatically. Do not reply.</span>
                    </div>
                </center>
                """ };
            Array.ForEach(attachments, file => builder.Attachments.Add(file.Name, file.Content, ContentType.Parse(file.ContentType)));

            message.Body = builder.ToMessageBody();
            await smtpClient.SendAsync(message);
        }

        public async Task SendEmailAsync(string to, string subject, string body, IEnumerable<FileData> attachments)
            => await SendEmailAsync(to, subject, body, [.. attachments]);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    smtpClient.Disconnect(true);
                    smtpClient.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
