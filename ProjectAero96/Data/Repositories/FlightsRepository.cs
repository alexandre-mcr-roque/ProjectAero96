using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;
using ProjectAero96.Helpers;
using ProjectAero96.Models;
using Syncfusion.EJ2.Linq;

namespace ProjectAero96.Data.Repositories
{
    public class FlightsRepository : IFlightsRepository
    {
        private readonly DataContext dataContext;

        public FlightsRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<ICollection<Flight>> GetAllFlightsAsync()
        {
            var flights = await dataContext.Flights.AsNoTracking()
                                                   .Include(f => f.FlightStops)
                                                   .ToListAsync();

            foreach (var flight in flights)
            {
                flight.FlightStops = flight.FlightStops.OrderBy(fs => fs.StopIndex).ToList();
            }
            return flights;
        }

        public async Task<ICollection<FlightViewModel>> GetAllFlightViewModelsAsync()
        {
            var flights = await GetAllFlightsAsync();
            return flights.ToFlightViewModels().ToList();
        }

        public async Task<ICollection<FlightViewModel>> GetFlightViewModelsWithStopsAsync(int fromCityId, int toCityId)
        {
            var flights = await GetAllFlightsAsync();
            return flights
                .Select(f => new FlightViewModel
                {
                    Id = f.Id,
                    DaysOfWeek = f.DaysOfWeek,
                    DepartureTime = f.DepartureTime,
                    ReturnTime = f.ReturnTime,
                    PricePerTime = f.PricePerTime,
                    ChildPriceModifier = f.ChildPriceModifier,
                    BabyPriceModifier = f.BabyPriceModifier,
                    AirplaneId = f.AirplaneId,
                    Airplane = f.Airplane?.ToAirplaneViewModel() ?? null,
                    FlightStops = f.FlightStops,
                    Deleted = f.Deleted
                })
                .Where(f =>
                {
                    var stops = (List<FlightStop>)f.FlightStops;
                    int fromIndex = -1;
                    int toIndex = -1;
                    foreach (var stop in stops)
                    {
                        if (stop.CityId == fromCityId) fromIndex = stop.StopIndex;
                        if (stop.CityId == toCityId) toIndex = stop.StopIndex;
                    }
                    bool isValid = fromIndex != -1 && toIndex != -1 && (f.HasReturn || fromIndex < toIndex);

                    f.Price = 0;
                    var filteredStops = new List<FlightStop>(stops.Count);
                    if (toIndex > fromIndex)
                    {
                        f.DepartureTime = f.ReturnTime!.Value;
                        for (int i = toIndex; i > fromIndex; i--)
                        {
                            var stop = stops[i];
                            filteredStops.Add(stop);
                            var time = stop.FromLastStop.GetValueOrDefault().TotalHours;
                            f.Price += f.PricePerTime * (decimal)time;
                        }
                    }
                    else
                    {
                        for (int i = fromIndex; i < toIndex; i++)
                        {
                            var stop = stops[i];
                            filteredStops.Add(stop);
                            var time = stop.ToNextStop.GetValueOrDefault().TotalHours;
                            f.Price += f.PricePerTime * (decimal)time;
                        }
                    }
                    f.ReturnTime = null; // Reset HasReturn since we are filtering flights
                    f.FlightStops = filteredStops;
                    return isValid;
                })
                .ToList();
        }

        public async Task<Flight?> GetFlightAsync(int id)
        {
            var flight = await dataContext.Flights.AsNoTracking()
                                                   .Include(f => f.FlightStops)
                                                   .FirstOrDefaultAsync(f => f.Id == id);

            if (flight != null) flight.FlightStops = flight.FlightStops.OrderBy(fs => fs.StopIndex).ToList();
            return flight;
        }

        public async Task<ICollection<City>> GetCitiesAsync()
        {
            return await dataContext.Cities.AsNoTracking().ToListAsync();
        }
    }
}
