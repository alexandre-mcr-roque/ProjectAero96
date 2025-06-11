using Microsoft.AspNetCore.Identity;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Helpers
{
    public interface IUserHelper
    {
        Task<int> GetUserCountAsync();
        Task<ICollection<User>> GetUsersAsync();
        Task<ICollection<User>> GetUsersWithRoleAsync();
        Task<User?> FindUserByIdAsync(string userId, bool includeRole = false);
        Task<User?> FindUserByEmailAsync(string email, bool includeRole = false);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task<string> GenerateVerifyEmailTokenAsync(User user);
        Task<IdentityResult> VerifyEmailAsync(User user, string token);
        Task<SignInResult> SignInAsync(User user, string password, bool isPersistent);
        Task SignOutAsync();
        Task CreateRoleAsync(Roles role);
        Task<bool> IsUserInRoleAsync(User user, Roles role);
        Task<IdentityResult> AddUserToRoleAsync(User user, Roles role);
    }
}
