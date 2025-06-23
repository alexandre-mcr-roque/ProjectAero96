using ProjectAero96.Data.Entities;
using ProjectAero96.Models;

namespace ProjectAero96.Data.Repositories
{
    public interface IFlightsRepository
    {
        Task<ICollection<Flight>> GetAllFlightsAsync();
        Task<ICollection<FlightViewModel>> GetAllFlightViewModelsAsync();
        Task<ICollection<FlightViewModel>> GetFlightViewModelsWithStopsAsync(int fromCityId, int toCityId);
        Task<Flight?> GetFlightAsync(int id);
        Task<ICollection<City>> GetCitiesAsync();
    }
}
