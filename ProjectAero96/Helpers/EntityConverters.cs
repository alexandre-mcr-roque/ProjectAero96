using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProjectAero96.Data.Entities;
using ProjectAero96.Models;
using System.Threading.Tasks;

namespace ProjectAero96.Helpers
{
    public static class EntityConverters
    {
        //====================================================================
        // User Converters
        //====================================================================
        public static async Task<AccountViewModel?> ToAccountViewModelAsync(this Task<User?> userT)
        {
            var user = await userT;
            if (user == null) return null;
            return user.ToAccountViewModel();
        }

        public static AccountViewModel ToAccountViewModel(this User user)
        {
            return new AccountViewModel
            {
                Id = user.Id,
                Email = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                Country = user.Country
            };
        }

        public static async Task<UserViewModel?> ToUserViewModelAsync(this Task<User?> userT)
        {
            var user = await userT;
            if (user == null) return null;
            return user.ToUserViewModel();
        }

        public static UserViewModel ToUserViewModel(this User user)
        {
            var model = new UserViewModel
            {
                Id = user.Id,
                Email = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                Country = user.Country,
                Deleted = user.Deleted
            };
            Roles roles = Roles.None;
            foreach (var role in user.Roles.Select(ur => Enum.Parse<Roles>(ur.Role.Name!)))
            {
                roles |= role;
                switch (role)
                {
                    case Roles.Admin:
                        model.IsAdmin = true;
                        break;
                    case Roles.Employee:
                        model.IsEmployee = true;
                        break;
                    case Roles.Client:
                        model.IsClient = true;
                        break;
                }
            }
            model.Roles = roles.ToString();
            return model;
        }

        public static ICollection<UserViewModel> ToUserViewModels(this ICollection<User> users)
        {
            return users.Select(u => u.ToUserViewModel())
                        .ToList();
        }

        public static ICollection<UserRole> ToUserRoles(this IdentityRole role, User user)
        {
            return [ new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            }];
        }

        public static ICollection<UserRole> ToUserRoles(this IEnumerable<IdentityRole> roles, User user)
        {
            var result = new List<UserRole>();
            foreach (var role in roles)
            {
                result.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
            return result;
        }

        public static async Task<ICollection<UserRole>> ToUserRolesAsync(this Task<IEnumerable<IdentityRole>> rolesT, User user)
        {
            var roles = await rolesT;
            return roles.Any() ? ToUserRoles(roles, user) : [];
        }

        //====================================================================
        // Airplane Converters
        //====================================================================
        public static async Task<AirplaneViewModel?> ToAirplaneViewModelAsync(this Task<Airplane?> airplaneT)
        {
            var airplane = await airplaneT;
            if (airplane == null) return null;
            return airplane.ToAirplaneViewModel();
        }

        public static AirplaneViewModel ToAirplaneViewModel(this Airplane airplane)
        {
            return new AirplaneViewModel
            {
                Id = airplane.Id,
                Airline = airplane.Airline,
                Description = airplane.Description,
                FCSeats = airplane.FCSeats,
                ESeats = airplane.ESeats,
                AirlineImageId = airplane.AirlineImageId,
                AirplaneModelId = airplane.AirplaneModelId,
                AirplaneModel = airplane.AirplaneModel,
                Deleted = airplane.Deleted
            };
        }

        public static ICollection<AirplaneViewModel> ToAirplaneViewModels(this ICollection<Airplane> airplanes)
        {
            return airplanes.Select(a => a.ToAirplaneViewModel())
                            .ToList();
        }

        public static async Task<ModelAirplaneViewModel?> ToModelAirplaneViewModelAsync(this Task<ModelAirplane?> airplaneModelT)
        {
            var model = await airplaneModelT;
            if (model == null) return null;
            return model.ToModelAirplaneViewModel();
        }

        public static ModelAirplaneViewModel ToModelAirplaneViewModel(this ModelAirplane airplaneModel)
        {
            return new ModelAirplaneViewModel
            {
                Id = airplaneModel.Id,
                ModelName = airplaneModel.ModelName,
                PricePerTime = airplaneModel.PricePerTime,
                MaxSeats = airplaneModel.MaxSeats,
                Deleted = airplaneModel.Deleted
            };
        }

        public static ICollection<ModelAirplaneViewModel> ToModelAirplaneViewModels(this ICollection<ModelAirplane> airplaneModels)
        {
            return airplaneModels.Select(m => m.ToModelAirplaneViewModel())
                                 .ToList();
        }

        //====================================================================
        // City Converters
        //====================================================================
        public static async Task<CityViewModel?> ToCityViewModelAsync(this Task<City?> cityT)
        {
            var city = await cityT;
            if (city == null) return null;
            return city.ToCityViewModel();
        }

        public static CityViewModel ToCityViewModel(this City city)
        {
            return new CityViewModel
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                Deleted = city.Deleted
            };
        }

        public static ICollection<CityViewModel> ToCityViewModels(this ICollection<City> cities)
        {
            return cities.Select(c => c.ToCityViewModel())
                         .ToList();
        }
    }
}
