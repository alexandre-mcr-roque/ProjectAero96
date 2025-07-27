using ProjectAero96.Data.Entities;
using ProjectAero96.Models;

namespace ProjectAero96.Helpers
{
    public interface IFileHelper
    {
        Task<IMailFileModel> GenerateTicketPdfAsync(FlightTicket ticket);
        Task<IMailFileModel> GenerateInvoicePdfAsync(Invoice invoice);
    }
}
