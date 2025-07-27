using ProjectAero96.Data.Entities;
using ProjectAero96.Models;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

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

        public async Task<IMailFileModel> GenerateTicketPdfAsync(FlightTicket ticket)
        {
            // Create PDF document
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            double y = 40;
            gfx.DrawString("Aero96", FontCompany, XBrushes.DarkBlue, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 35;
            gfx.DrawString("Flight Ticket", FontTitle, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            void DrawField(string label, string value)
            {
                gfx.DrawString(label, FontLabel, XBrushes.Black, 40, y);
                gfx.DrawString(value, FontValue, XBrushes.Black, 180, y);
                y += 25;
            }

            DrawField("Ticket ID:", ticket.Id.ToString());
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
            return await Task.FromResult(new PdfFile(fileName, pdfBytes));
        }

        public async Task<IMailFileModel> GenerateInvoicePdfAsync(Invoice invoice)
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

            DrawField("Invoice ID:", invoice.Id.ToString());
            DrawField("First Name:", invoice.FirstName);
            DrawField("Last Name:", invoice.LastName);
            DrawField("Email:", invoice.Email);
            DrawField("Address 1:", invoice.Address1);
            DrawField("Address 2:", string.IsNullOrWhiteSpace(invoice.Address2) ? "N/A" : invoice.Address2);
            DrawField("City:", invoice.City);
            DrawField("Country:", invoice.Country);
            DrawField("Departure Date:", invoice.DepartureDate.ToString("yyyy-MM-dd HH:mm"));
            DrawField("Flight ID:", invoice.FlightId.ToString());
            DrawField("Total Price:", $"${invoice.TotalPrice:F2}");

            using var stream = new MemoryStream();
            document.Save(stream, false);
            var pdfBytes = stream.ToArray();
        
            var fileName = $"Invoice_{invoice.Id}.pdf";
            return await Task.FromResult(new PdfFile(fileName, pdfBytes));
        }
    }
}
