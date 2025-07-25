﻿using Microsoft.AspNetCore.Identity;
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

        public async Task<IdentityResult> AddUserAsync(User user)
        {
            user.RequiresPasswordChange = true; // Set RequiresPasswordChange to true by default
            return await userManager.CreateAsync(user);
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> SetUserDeleted(User user, bool deleted = true)
        {
            user.Deleted = deleted;
            return await userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<string> GenerateVerifyEmailTokenAsync(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> VerifyEmailAsync(User user, string token)
        {
            return await userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateResetPasswordTokenAsync(User user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return await userManager.ResetPasswordAsync(user, token, newPassword);
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

        public async Task<IEnumerable<IdentityRole>> GetRolesAsync(Roles roles)
        {
            var result = new List<IdentityRole>();
            foreach (var role in Enum.GetValues<Roles>())
            {
                if (roles.HasFlag(role) && role != Roles.None)
                {
                    var userRole = await roleManager.FindByNameAsync(role.ToString());
                    if (userRole != null)
                        result.Add(userRole);
                }
            }
            return result;
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
