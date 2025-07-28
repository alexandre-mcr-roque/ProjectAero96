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
        Task<IdentityResult> AddUserAsync(User user);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> SetUserDeleted(User user, bool deleted = true);
        Task<SignInResult> ValidatePasswordAsync(User user, string password);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<string> GenerateVerifyEmailTokenAsync(User user);
        Task<IdentityResult> VerifyEmailAsync(User user, string token);
        Task<string> GenerateResetPasswordTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);
        Task<SignInResult> SignInAsync(User user, string password, bool isPersistent);
        Task SignOutAsync();
        Task CreateRoleAsync(Roles role);
        Task<bool> IsUserInRoleAsync(User user, Roles role);
        Task<IdentityResult> AddUserToRoleAsync(User user, Roles role);
        Task<IEnumerable<IdentityRole>> GetRolesAsync(Roles roles);

        //==========================================================
        // Passthrough methods
        //==========================================================
        /// <inheritdoc cref="UserManager{TUser}.NormalizeEmail(string?)"/>
        string? NormalizeEmail(string? email);
        /// <inheritdoc cref="UserManager{TUser}.NormalizeName(string?)"/>
        string? NormalizeName(string? name);
    }
}
