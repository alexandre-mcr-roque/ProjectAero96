using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data.Repositories
{
    public interface IFlightsRepository
    {
        Task<ICollection<Flight>> GetAllFlightsAsync(int? fromCity, int? toCity);
        Task<ICollection<Flight>> GetFutureFlightsAsync(int? fromCity, int? toCity);
        Task<Flight?> GetFlightAsync(int id);
        Task<bool> AddFlightAsync(Flight flight);
        Task<int> AddFlightsAsync(ICollection<Flight> flights);
        Task<bool> UpdateFlightAsync(Flight flight);
        Task<bool> DeleteFlightAsync(Flight flight);
        Task<bool> HasFlightTicketsAsync(Flight flight);
        Task<ICollection<Flight>> GetBookedFlightsOfUserAsync(User user);
        /// <summary>
        /// Options:<br/>
        /// 0 -> All invoices<br/>
        /// 1 -> Invoices of past flights<br/>
        /// 2 -> Invoices of future flights
        /// </summary>
        Task<ICollection<Invoice>> GetInvoicesOfUserAsync(User user, byte option = 1, bool includeTickets = false);
        Task<Invoice?> GetInvoiceAsync(int id);
        Task<bool> RegisterFlightTicketsAsync(Invoice invoice);
        Task<ICollection<SelectListItem>> GetCitySelectListItemsAsync(bool includeEmpty = false);
        Task<ICollection<SelectListItem>> GetAirplaneSelectListItemsAsync();
        Task<City?> GetCityAsync(int id);
    }
}
