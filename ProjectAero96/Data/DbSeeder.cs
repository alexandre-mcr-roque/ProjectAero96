using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjectAero96.Data.Entities;
using ProjectAero96.Helpers;
using System.Data;

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
                if (role == Roles.None) continue;
                await userHelper.CreateRoleAsync(role);
            }

            var adminRole = dataContext.Roles.FirstOrDefault(r => r.Name == Roles.Admin.ToString());
            if (adminRole == null) throw new Exception("Failed to create role 'Admin'");
            
            IdentityResult result;
            var user = await userHelper.FindUserByEmailAsync(adminEmail);
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
                user.Roles = [new UserRole { User = user, Role = adminRole }];
                result = await userHelper.AddUserAsync(user, adminPassword);
                if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
#if DEBUG
                if (dataContext.Users.Count() == 1) // only create test users if the admin user is the only one
                {
                    System.Diagnostics.Debug.WriteLine("""
                        ===================================================================================================================
                        SeedDb: Creating test users, this may take a while...
                        ===================================================================================================================
                        """);
                    CreateTestUsers(20000, Roles.Client);
                    CreateTestUsers(200, Roles.Employee);
                }
#endif
            }
            result = await userHelper.AddUserToRoleAsync(user, Roles.Admin);
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
        }
#if DEBUG
        private readonly string testPassword = "12345Abc";
        public void CreateTestUsers(int amount, Roles role)
        {
            string roleName = role.ToString();
            var iRole = dataContext.Roles.FirstOrDefault(r => r.Name == roleName);
            if (iRole == null) throw new Exception($"Failed to create role '{roleName}'");
            string _email = $"test.{roleName.ToLowerInvariant()}{{0}}@mail.com";
            string name = $"Test {roleName}";
            var hasher = new PasswordHasher<User>();
            var users = new User[amount];
            Parallel.For(1, amount + 1, i =>
            {
                var email = _email.FormatWith(i);
                var user = new User
                {
                    UserName = email,
                    FirstName = name,
                    LastName = i.ToString(),
                    Email = email,
                    EmailConfirmed = true,  // manually set email as confirmed
                    Address1 = "(Address)",
                    City = "(City)",
                    Country = "(Country)",
                    // due to manually adding user to DbSet, it is required to set the normalized values
                    NormalizedUserName = userHelper.NormalizeName(email),
                    NormalizedEmail = userHelper.NormalizeEmail(email)
                };
                user.PasswordHash = hasher.HashPassword(user, testPassword);
                user.Roles = [new UserRole { User = user, Role = iRole }];
                System.Diagnostics.Debug.WriteLine($"SeedDb: Prepared user with name {user.FirstName} {user.LastName} and email {email}");
                users[i - 1] = user;
            });
            dataContext.Users.AddRange(users);
            dataContext.SaveChanges();
        }
#endif
    }
}
