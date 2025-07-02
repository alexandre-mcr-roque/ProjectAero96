using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data.Repositories
{
    public class AirplanesRepository : IAirplanesRepository
    {
        private readonly DataContext dataContext;

        public AirplanesRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<Airplane?> GetAirplaneAsync(int id)
        {
            return await dataContext.Airplanes.AsNoTracking()
                                              .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Airplane?> GetAirplaneWithModelAsync(int id)
        {
            return await dataContext.Airplanes.AsNoTracking()
                                              .Include(a => a.AirplaneModel)
                                              .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ICollection<Flight>> GetAllFlightsFromAirplaneAsync(int id)
        {
            return await dataContext.Flights.AsNoTracking()
                                            .Where(f => f.AirplaneId == id)
                                            .ToListAsync();
        }
    }
}
