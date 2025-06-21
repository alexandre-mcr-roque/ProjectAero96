using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data.Repositories
{
    public interface IAdminRepository
    {
        Task<ICollection<Airplane>> GetAirplanesAsync();
        Task<Airplane?> GetAirplaneAsync(int id);
        Task<bool> AddAirplaneAsync(Airplane airplane);
        Task<bool> UpdateAirplaneAsync(Airplane airplane);
        Task<bool> DeleteAirplaneAsync(Airplane airplane);
        Task<bool> IsAirplaneInUse(Airplane airplane);

        Task<ICollection<City>> GetCitiesAsync();
        Task<City?> GetCityAsync(int id);
        Task<bool> AddCityAsync(City city);
        Task<bool> UpdateCityAsync(City city);
        Task<bool> DeleteCityAsync(City city);
        Task<bool> IsCityInUse(City city);

        Task<ICollection<ModelAirplane>> GetAirplaneModelsAsync();
        Task<ICollection<SelectListItem>> GetAirplaneModelSelectItemsAsync();
        Task<ModelAirplane?> GetAirplaneModelAsync(int id);
        Task<bool> AddAirplaneModelAsync(ModelAirplane airplaneModel);
        Task<bool> UpdateAirplaneModelAsync(ModelAirplane airplaneModel);
        Task<bool> DeleteAirplaneModelAsync(ModelAirplane airplaneModel);
        Task<bool> IsAirplaneModelInUse(ModelAirplane airplaneModel);
    }
}