using Microsoft.AspNetCore.Identity;
using ProjectAero96.Data;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Helpers
{
    public interface IUserHelper
    {
        Task<User?> FindUserByIdAsync(string userId);
        Task<User?> FindUserByEmailAsync(string email);
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
