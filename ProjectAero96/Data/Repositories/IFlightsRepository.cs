using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data.Repositories
{
    public interface IFlightsRepository
    {
        Task<ICollection<Flight>> GetAllFlightsAsync(int? fromCity, int? toCity);
        Task<Flight?> GetFlightAsync(int id);
        Task<bool> AddFlightAsync(Flight flight);
        Task<ICollection<SelectListItem>> GetCitySelectListItemsAsync(bool includeEmpty = false);
        Task<ICollection<SelectListItem>> GetAirplaneSelectListItemsAsync();
        Task<City?> GetCityAsync(int id);
    }
}
