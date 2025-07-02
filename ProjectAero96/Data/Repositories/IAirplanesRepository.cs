using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data.Repositories
{
    public interface IAirplanesRepository
    {
        Task<Airplane?> GetAirplaneAsync(int id);
        Task<Airplane?> GetAirplaneWithModelAsync(int id);
        Task<ICollection<Flight>> GetAllFlightsFromAirplaneAsync(int id);
    }
}
