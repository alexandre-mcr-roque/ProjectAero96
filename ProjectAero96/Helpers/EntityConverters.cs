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
    }
}
