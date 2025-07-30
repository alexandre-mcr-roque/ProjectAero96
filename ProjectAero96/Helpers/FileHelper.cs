using ProjectAero96.Data.Entities;
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

        private class PdfFile(string fileName, byte[] content) : FileData
        {
            public override string Name { get; } = fileName;
            public override byte[] Content { get; } = content;
            public override string ContentType => "application/pdf";
        }

        private static void DrawTicketPdf(FlightTicket ticket, PdfPage page)
        {
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
        }

        private FileData GenerateTicketPdf(FlightTicket ticket)
        {
            // Create PDF document
            var document = new PdfDocument();
            var page = document.AddPage();
            DrawTicketPdf(ticket, page);

            using var stream = new MemoryStream();
            document.Save(stream, false);
            var pdfBytes = stream.ToArray();

            var fileName = $"FlightTicket_{ticket.Id}.pdf";
            return new PdfFile(fileName, pdfBytes);
        }
        public async Task<FileData> GenerateTicketPdfAsync(FlightTicket ticket)
            => await Task.FromResult(GenerateTicketPdf(ticket));

        private FileData GenerateTicketPdfs(IEnumerable<FlightTicket> tickets, DateTime dateTime)
        {
            // Create PDF document
            var document = new PdfDocument();
            foreach (var ticket in tickets)
            {
                var page = document.AddPage();
                DrawTicketPdf(ticket, page);
            }

            using var stream = new MemoryStream();
            document.Save(stream, false);
            var pdfBytes = stream.ToArray();

            var fileName = $"FlightTickets_{dateTime:yyyy-MM-dd_HH\\hmm\\m}.pdf";
            return new PdfFile(fileName, pdfBytes);
        }
        public async Task<FileData> GenerateTicketPdfsAsync(IEnumerable<FlightTicket> tickets, DateTime dateTime)
            => await Task.FromResult(GenerateTicketPdfs(tickets, dateTime));

        private FileData CombineTicketPdfs(IEnumerable<FileData> tickets, DateTime dateTime)
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

            var fileName = $"FlightTickets_{dateTime:yyyy-MM-dd_HH\\hmm\\m}.pdf";
            return new PdfFile(fileName, combinedBytes);
        }
        public async Task<FileData> CombineTicketPdfsAsync(IEnumerable<FileData> tickets, DateTime dateTime)
            => await Task.FromResult(CombineTicketPdfs(tickets, dateTime));

        private FileData GenerateInvoicePdf(Invoice invoice)
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

            DrawField("Payment Date:", invoice.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
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
            DrawField("Flight Duration:", invoice.FlightDuration);
            y += 10;
            DrawField("Total Price:", $"${invoice.TotalPrice:F2}");

            using var stream = new MemoryStream();
            document.Save(stream, false);
            var pdfBytes = stream.ToArray();

            var fileName = $"Invoice_{invoice.Id}.pdf";
            return new PdfFile(fileName, pdfBytes);
        }
        public async Task<FileData> GenerateInvoicePdfAsync(Invoice invoice)
            => await Task.FromResult(GenerateInvoicePdf(invoice));
    }
}
