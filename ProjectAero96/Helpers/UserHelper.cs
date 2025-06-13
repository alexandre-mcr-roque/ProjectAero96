using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserHelper(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<int> GetUserCountAsync() => await userManager.Users.CountAsync();

        public async Task<ICollection<User>> GetUsersAsync()
        {
            return await userManager.Users.AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<User>> GetUsersWithRoleAsync()
        {
            return await userManager.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<User?> FindUserByIdAsync(string userId, bool includeRole = false)
        {
            if (includeRole)
            {
                return await userManager.Users
                    .Include(u => u.Roles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<User?> FindUserByEmailAsync(string email, bool includeRole = false)
        {
            if (includeRole)
            {
                return await userManager.Users
                    .Include(u => u.Roles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserName == email); // UserName == Email
            }
            return await userManager.FindByNameAsync(email);    // UserName == Email
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }

        public async Task<string> GenerateVerifyEmailTokenAsync(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> VerifyEmailAsync(User user, string token)
        {
            return await userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<SignInResult> SignInAsync(User user, string password, bool isPersistent)
        {
            return await signInManager.PasswordSignInAsync(user, password, isPersistent, false);
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task CreateRoleAsync(Roles role)
        {
            string roleName = role.ToString();
            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }
        }

        public async Task<bool> IsUserInRoleAsync(User user, Roles role)
        {
            return await userManager.IsInRoleAsync(user, role.ToString());
        }

        public async Task<IdentityResult> AddUserToRoleAsync(User user, Roles role)
        {
            if (await IsUserInRoleAsync(user, role)) return IdentityResult.Success;
            return await userManager.AddToRoleAsync(user, role.ToString());
        }

        public async Task<IdentityResult> SetUserDeleted(User user, bool deleted = true)
        {
            user.Deleted = deleted;
            return await userManager.UpdateAsync(user);
        }

        //==========================================================
        // Passthrough methods
        //==========================================================
        /// <inheritdoc cref="UserManager{TUser}.NormalizeEmail(string?)"/>
        public string? NormalizeEmail(string? email) => userManager.NormalizeEmail(email);
        /// <inheritdoc cref="UserManager{TUser}.NormalizeName(string?)"/>
        public string? NormalizeName(string? name) => userManager.NormalizeName(name);
    }
}
