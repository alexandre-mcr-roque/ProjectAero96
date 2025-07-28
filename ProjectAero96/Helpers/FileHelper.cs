using ProjectAero96.Data.Entities;
using ProjectAero96.Models;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.IO;

namespace ProjectAero96.Helpers
{
    public class FileHelper : IFileHelper
    {
        // Fonts
        private static readonly XFont FontCompany = new XFont("Arial", 20, XFontStyle.Bold);
        private static readonly XFont FontTitle = new XFont("Arial", 16, XFontStyle.Bold);
        private static readonly XFont FontLabel = new XFont("Arial", 12, XFontStyle.Bold);
        private static readonly XFont FontValue = new XFont("Arial", 12, XFontStyle.Regular);

        private class PdfFile(string fileName, byte[] content) : IMailFileModel
        {
            public string FileName { get; } = fileName;
            public byte[] Content { get; } = content;
            public string ContentType => "application/pdf";
        }

        private IMailFileModel GenerateTicketPdf(FlightTicket ticket)
        {
            // Create PDF document
            var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A5;
            page.Orientation = PdfSharpCore.PageOrientation.Landscape;
            var gfx = XGraphics.FromPdfPage(page);

            double y = 40;
            gfx.DrawString("Aero96", FontCompany, XBrushes.DarkBlue, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 35;
            gfx.DrawString("Flight Ticket", FontTitle, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 80;

            void DrawField(string label, string value)
            {
                gfx.DrawString(label, FontLabel, XBrushes.Black, 40, y);
                gfx.DrawString(value, FontValue, XBrushes.Black, 180, y);
                y += 25;
            }

            DrawField("First Name:", ticket.FirstName);
            DrawField("Last Name:", ticket.LastName);
            DrawField("Age:", ticket.Age.ToString());
            DrawField("Email:", ticket.Email);
            DrawField("Seat Number:", ticket.SeatNumber ?? "N/A");
            DrawField("Price:", $"${ticket.Price:F2}");

            using var stream = new MemoryStream();
            document.Save(stream, false);
            var pdfBytes = stream.ToArray();

            var fileName = $"FlightTicket_{ticket.Id}.pdf";
            return new PdfFile(fileName, pdfBytes);
        }
        public async Task<IMailFileModel> GenerateTicketPdfAsync(FlightTicket ticket)
            => await Task.FromResult(GenerateTicketPdf(ticket));

        private IMailFileModel GenerateInvoicePdf(Invoice invoice)
        {
            // Create PDF document
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            double y = 40;
            gfx.DrawString("Aero96", FontCompany, XBrushes.DarkBlue, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 35;
            gfx.DrawString("Invoice", FontTitle, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            void DrawField(string label, string value)
            {
                gfx.DrawString(label, FontLabel, XBrushes.Black, 40, y);
                gfx.DrawString(value, FontValue, XBrushes.Black, 180, y);
                y += 25;
            }

            DrawField("Created At:", invoice.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
            DrawField("First Name:", invoice.FirstName);
            DrawField("Last Name:", invoice.LastName);
            DrawField("Email:", invoice.Email);
            DrawField("Address:", invoice.FullAddress);
            DrawField("City:", invoice.City);
            DrawField("Country:", invoice.Country);
            y += 40;
            gfx.DrawString("Flight Details", FontTitle, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;
            DrawField("Departure:", $"{invoice.DepartureCity} at {invoice.DepartureDate:yyyy/MM/dd HH:mm}");
            DrawField("Arrival:", invoice.ArrivalCity);
            DrawField("Expected Flight Duration:", invoice.FlightDuration);
            y += 10;
            DrawField("Total Price:", $"${invoice.TotalPrice:F2}");

            using var stream = new MemoryStream();
            document.Save(stream, false);
            var pdfBytes = stream.ToArray();

            var fileName = $"Invoice_{invoice.Id}.pdf";
            return new PdfFile(fileName, pdfBytes);
        }
        public async Task<IMailFileModel> GenerateInvoicePdfAsync(Invoice invoice)
            => await Task.FromResult(GenerateInvoicePdf(invoice));

        private IMailFileModel CombineTicketPdfs(IEnumerable<IMailFileModel> tickets, DateTime dateTime)
        {
            var outputDocument = new PdfDocument();
            foreach (var ticket in tickets)
            {
                using var ms = new MemoryStream(ticket.Content);
                var inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import);

                // Import all pages (should be 1 per ticket)
                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    outputDocument.AddPage(inputDocument.Pages[i]);
                }
            }

            using var outStream = new MemoryStream();
            outputDocument.Save(outStream, false);
            var combinedBytes = outStream.ToArray();

            var fileName = $"FlightTickets_{dateTime:yyyyMMddHHmm}.pdf";
            return new PdfFile(fileName, combinedBytes);
        }
        public async Task<IMailFileModel> CombineTicketPdfsAsync(IEnumerable<IMailFileModel> tickets, DateTime dateTime)
            => await Task.FromResult(CombineTicketPdfs(tickets, dateTime));
    }
}
