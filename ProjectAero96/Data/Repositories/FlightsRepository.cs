using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;
using ProjectAero96.Helpers;

namespace ProjectAero96.Data.Repositories
{
    public class FlightsRepository : IFlightsRepository
    {
        private readonly DataContext dataContext;

        public FlightsRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<ICollection<Flight>> GetAllFlightsAsync(int? departureCity, int? arrivalCity)
        {
            if (departureCity != null && arrivalCity != null && departureCity.Value == arrivalCity.Value)
            {
                return []; // No flights if departure and arrival are the same
            }
            return await dataContext.Flights.AsNoTracking()
                                            .Include(f => f.DepartureCity)
                                            .Include(f => f.ArrivalCity)
                                            .Where(f => !f.Deleted
                                                        && (!departureCity.HasValue || departureCity == 0 || f.DepartureCityId == departureCity.Value)
                                                        && (!arrivalCity.HasValue || arrivalCity == 0 || f.ArrivalCityId == arrivalCity.Value))
                                            .ToListAsync();
        }

        public async Task<ICollection<Flight>> GetFutureFlightsAsync(int? departureCity, int? arrivalCity)
        {
            if (departureCity != null && arrivalCity != null && departureCity.Value == arrivalCity.Value)
            {
                return []; // No flights if departure and arrival are the same
            }
            return await dataContext.Flights.AsNoTracking()
                                            .Include(f => f.DepartureCity)
                                            .Include(f => f.ArrivalCity)
                                            .Where(f => !f.Deleted
                                                        && f.DepartureDate > DateTime.UtcNow
                                                        && (!departureCity.HasValue || departureCity == 0 || f.DepartureCityId == departureCity.Value)
                                                        && (!arrivalCity.HasValue || arrivalCity == 0 || f.ArrivalCityId == arrivalCity.Value))
                                            .ToListAsync();
        }

        public async Task<Flight?> GetFlightAsync(int id)
        {
            return await dataContext.Flights.AsNoTracking()
                                            .Include(f => f.DepartureCity)
                                            .Include(f => f.ArrivalCity)
                                            .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> AddFlightAsync(Flight flight)
        {
            dataContext.Flights.Add(flight);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<int> AddFlightsAsync(ICollection<Flight> flights)
        {
            dataContext.Flights.AddRange(flights);
            return await dataContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateFlightAsync(Flight flight)
        {
            dataContext.Flights.Update(flight);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFlightAsync(Flight flight)
        {
            dataContext.Flights.Remove(flight);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> HasFlightTicketsAsync(Flight flight)
        {
            return await dataContext.Invoices.AsNoTracking()
                .AnyAsync(i => i.FlightId == flight.Id
                                && i.DepartureDate > DateTime.UtcNow);
        }

        public async Task<ICollection<Flight>> GetBookedFlightsOfUserAsync(User user)
        {
            return await dataContext.Flights.AsNoTracking()
                .Include(f => f.DepartureCity)
                .Include(f => f.ArrivalCity)
                .Where(f => f.Invoices.Any(i => i.Email == user.Email))
                .ToListAsync();
        }

        public async Task<bool> RegisterFlightTicketsAsync(Invoice invoice)
        {
            dataContext.Invoices.Add(invoice);
            return await dataContext.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<SelectListItem>> GetCitySelectListItemsAsync(bool includeEmpty = false)
        {
            var cities = await dataContext.Cities.AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => c.ToCityViewModel())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ToString()
                })
                .ToListAsync();

            if (includeEmpty)
            {
                cities.Insert(0, new SelectListItem
                {
                    Value = "0",
                    Text = "Select a city"
                });
            }
            return cities;
        }

        public async Task<ICollection<SelectListItem>> GetAirplaneSelectListItemsAsync()
        {
            var airplanes = await dataContext.Airplanes.AsNoTracking()
                .Include(a => a.AirplaneModel)
                .Select(a => a.ToAirplaneViewModel())
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.DescriptionStr
                })
                .ToListAsync();

            airplanes.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "Select an airplane"
            });
            return airplanes;
        }

        public async Task<City?> GetCityAsync(int id)
        {
            return await dataContext.Cities.AsNoTracking()
                                           .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
