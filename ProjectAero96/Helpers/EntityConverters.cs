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

        public static async Task<User> ToNewEntityAsync(this UserViewModel model, IUserHelper userHelper)
        {
            var user = new User
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
            if (model.IsAdmin)
            {
                var role = await userHelper.GetRolesAsync(Roles.Admin);
                user.Roles.Add(new UserRole { User = user, Role = role.First() });
            }
            if (model.IsEmployee)
            {
                var role = await userHelper.GetRolesAsync(Roles.Employee);
                user.Roles.Add(new UserRole { User = user, Role = role.First() });
            }
            if (model.IsClient)
            {
                var role = await userHelper.GetRolesAsync(Roles.Client);
                user.Roles.Add(new UserRole { User = user, Role = role.First() });
            }
            return user;
        }

        public static User ToNewEntity(this RegisterViewModel model)
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

        public static async Task<ICollection<UserRole>> ToUserRolesAsync(this Task<IEnumerable<IdentityRole>> rolesT, User user)
        {
            var roles = await rolesT;
            return roles.Any() ? ToUserRoles(roles, user) : [];
        }
    }
}
