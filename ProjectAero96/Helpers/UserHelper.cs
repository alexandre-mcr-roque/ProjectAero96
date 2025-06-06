﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data;
using ProjectAero96.Data.Entities;
using Syncfusion.EJ2.Linq;

namespace ProjectAero96.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext dataContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserHelper(
            DataContext dataContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public Task<int> GetUserCountEstimateAsync() => dataContext.GetUserCountEstimateAsync();

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await userManager.Users.AsNoTracking().ToListAsync();
        }

        // TODO test
        public async Task<IEnumerable<User>> GetUsersAsync(int pageIndex, int pageSize)
        {
            var users = userManager.Users.AsNoTracking();
            if (pageIndex > 1)
            {
                users = users.Skip(pageIndex * pageSize);
            }
            if (pageSize > 0)
            {
                users = users.Take(pageSize);
            }
            return await users.ToListAsync();
        }

        public async Task<User?> FindUserByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<User?> FindUserByEmailAsync(string email)
        {
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
    }
}
