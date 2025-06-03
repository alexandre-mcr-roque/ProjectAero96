using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext dataContext;

        public AdminRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await dataContext.Cities.AsNoTracking().ToListAsync();
        }

        public async Task<City?> GetCityAsync(int id)
        {
            return await dataContext.Cities.AsNoTracking()
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddCityAsync(City city)
        {
            dataContext.Cities.Add(city);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCityAsync(City city)
        {
            dataContext.Cities.Update(city);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCityAsync(City city)
        {
            dataContext.Cities.Remove(city);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ModelAirplane>> GetAirplaneModelsAsync()
        {
            return await dataContext.AirplaneModels.AsNoTracking().ToListAsync();
        }

        public async Task<ModelAirplane?> GetAirplaneModelAsync(int id)
        {
            return await dataContext.AirplaneModels.AsNoTracking()
                .Where(am => am.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddAirplaneModelAsync(ModelAirplane modelAirplane)
        {
            dataContext.AirplaneModels.Add(modelAirplane);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAirplaneModelAsync(ModelAirplane modelAirplane)
        {
            dataContext.AirplaneModels.Update(modelAirplane);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAirplaneModelAsync(ModelAirplane modelAirplane)
        {
            dataContext.AirplaneModels.Remove(modelAirplane);
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}
