using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;
using ProjectAero96.Helpers;

namespace ProjectAero96.Data
{
    public class DbSeeder
    {
        private readonly DataContext dataContext;
        private readonly IUserHelper userHelper;

        private readonly string adminEmail = "alexandre.mcr.roque@gmail.com";
        private readonly string adminPassword = "12345Abc";

        public DbSeeder(
            DataContext dataContext,
            IUserHelper userHelper)
        {
            this.dataContext = dataContext;
            this.userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await dataContext.Database.MigrateAsync();

            foreach (Roles role in Enum.GetValues(typeof(Roles)))
            {
                await userHelper.CreateRoleAsync(role);
            }

            IdentityResult result;
            var user = await userHelper.FindUserByEmailAsync("alexandre.mcr.roque@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    UserName = adminEmail,
                    FirstName = "Aero96",
                    LastName = "Administrator",
                    Email = adminEmail,
                    EmailConfirmed = true,  // manually set email as confirmed
                    Address1 = "(Address)",
                    City = "(City)",
                    Country = "(Country)"
                };
                result = await userHelper.AddUserAsync(user, adminPassword);
                if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
            }
            result = await userHelper.AddUserToRoleAsync(user, Roles.Admin);
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
        }
    }
}
