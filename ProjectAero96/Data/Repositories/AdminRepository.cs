using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;
using ProjectAero96.Helpers;

namespace ProjectAero96.Data.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext dataContext;
        private readonly IImageHelper imageHelper;

        public AdminRepository(DataContext dataContext, IImageHelper imageHelper)
        {
            this.dataContext = dataContext;
            this.imageHelper = imageHelper;
        }

        public async Task<ICollection<Airplane>> GetAirplanesAsync()
        {
            return await dataContext.Airplanes.AsNoTracking()
                .Include(a => a.AirplaneModel)
                .ToListAsync();
        }

        public async Task<Airplane?> GetAirplaneAsync(int id)
        {
            return await dataContext.Airplanes.AsNoTracking()
                .Include(a => a.AirplaneModel)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> AddAirplaneAsync(Airplane airplane)
        {
            dataContext.Airplanes.Add(airplane);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAirplaneAsync(Airplane airplane)
        {
            dataContext.Airplanes.Update(airplane);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAirplaneAsync(Airplane airplane)
        {
            if (airplane.AirlineImageId != null)
            {
                await imageHelper.DeleteAirlineImageAsync(airplane.AirlineImageId);
            }
            dataContext.Airplanes.Remove(airplane);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsAirplaneInUse(Airplane airplane)
        {
            return await dataContext.Flights.AnyAsync(f => f.AirplaneId == airplane.Id);
        }

        public async Task<ICollection<City>> GetCitiesAsync()
        {
            return await dataContext.Cities.AsNoTracking().ToListAsync();
        }

        public async Task<City?> GetCityAsync(int id)
        {
            return await dataContext.Cities.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
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

        public async Task<bool> IsCityInUse(City city)
        {
            return await dataContext.Flights.AnyAsync(f => f.DepartureCityId == city.Id
                                                        || f.ArrivalCityId == city.Id);
        }

        public async Task<ICollection<ModelAirplane>> GetAirplaneModelsAsync()
        {
            return await dataContext.AirplaneModels.AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<SelectListItem>> GetAirplaneModelSelectItemsAsync()
        {
            var models = await dataContext.AirplaneModels.AsNoTracking()
                .Select(am => am.ToModelAirplaneViewModel())
                .Select(am => new SelectListItem
                {
                    Value = am.Id.ToString(),
                    Text = am.ToString()
                })
                .ToListAsync();
            models.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "Select an airplane model"
            });
            return models;
        }

        public async Task<ModelAirplane?> GetAirplaneModelAsync(int id)
        {
            return await dataContext.AirplaneModels.AsNoTracking()
                .FirstOrDefaultAsync(am => am.Id == id);
        }

        public async Task<bool> AddAirplaneModelAsync(ModelAirplane airplaneModel)
        {
            dataContext.AirplaneModels.Add(airplaneModel);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAirplaneModelAsync(ModelAirplane airplaneModel)
        {
            dataContext.AirplaneModels.Update(airplaneModel);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAirplaneModelAsync(ModelAirplane airplaneModel)
        {
            dataContext.AirplaneModels.Remove(airplaneModel);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsAirplaneModelInUse(ModelAirplane airplaneModel)
        {
            return await dataContext.Airplanes.AnyAsync(a => a.AirplaneModelId == airplaneModel.Id);
        }
    }
}
