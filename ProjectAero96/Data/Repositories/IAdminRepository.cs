using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data.Repositories
{
    public interface IAdminRepository
    {
        Task<ICollection<City>> GetCitiesAsync();
        Task<City?> GetCityAsync(int id);
        Task<bool> AddCityAsync(City city);
        Task<bool> UpdateCityAsync(City city);
        Task<bool> DeleteCityAsync(City city);

        Task<ICollection<ModelAirplane>> GetAirplaneModelsAsync();
        Task<ModelAirplane?> GetAirplaneModelAsync(int id);
        Task<bool> AddAirplaneModelAsync(ModelAirplane modelAirplane);
        Task<bool> UpdateAirplaneModelAsync(ModelAirplane modelAirplane);
        Task<bool> DeleteAirplaneModelAsync(ModelAirplane modelAirplane);
    }
}