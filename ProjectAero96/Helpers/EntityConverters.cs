using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProjectAero96.Data.Entities;
using ProjectAero96.Models;

namespace ProjectAero96.Helpers
{
    public static class EntityConverters
    {
        //====================================================================
        // User Converters
        //====================================================================
        public static async Task<UserViewModel?> ToViewModelAsync(this Task<User?> userT, IUserHelper userHelper)
        {
            var user = await userT;
            if (user == null) return null;
            return user.ToViewModel(userHelper);
        }

        public static UserViewModel ToViewModel(this User user, IUserHelper userHelper)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Email = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                Country = user.Country,
                Deleted = user.Deleted,
                Roles = string.Join(',', user.Roles.Select(ur => Enum.Parse<Roles>(ur.Role?.Name ?? "None"))),
            };
        }

        public static ICollection<UserViewModel> ToViewModels(this ICollection<User> users, IUserHelper userHelper)
        {
            return users.Select(u => u.ToViewModel(userHelper))
                        .ToList();
        }

        public static User ToEntity(this UserViewModel model)
        {
            return new User
            {
                Id = model.Id,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Country = model.Country,
                Deleted = model.Deleted
            };
        }

        public static User ToEntity(this RegisterViewModel model)
        {
            return new User
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Country = model.Country
            };
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

        public static async Task<ICollection<UserRole>> ToUserRoles(this Task<IEnumerable<IdentityRole>> rolesT, User user)
        {
            var roles = await rolesT;
            return roles.Any() ? ToUserRoles(roles, user) : [];
        }
    }
}
