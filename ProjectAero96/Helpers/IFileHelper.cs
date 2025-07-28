using ProjectAero96.Data.Entities;
using ProjectAero96.Models;

namespace ProjectAero96.Helpers
{
    public interface IFileHelper
    {
        Task<FileData> GenerateTicketPdfAsync(FlightTicket ticket);
        Task<FileData> GenerateTicketPdfsAsync(IEnumerable<FlightTicket> tickets, DateTime dateTime);
        Task<FileData> CombineTicketPdfsAsync(IEnumerable<FileData> tickets, DateTime dateTime);
        Task<FileData> GenerateInvoicePdfAsync(Invoice invoice);
    }
}
