using Humanizer;
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
        private readonly IImageHelper imageHelper;
        private readonly string adminEmail = "alexandre.mcr.roque@gmail.com";
        private readonly string adminPassword = "12345Abc";

        public DbSeeder(
            DataContext dataContext,
            IUserHelper userHelper,
            IImageHelper imageHelper)
        {
            this.dataContext = dataContext;
            this.userHelper = userHelper;
            this.imageHelper = imageHelper;
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
                    BirthDate = new DateTime(2002, 9, 9),
                    Email = adminEmail,
                    EmailConfirmed = true,  // manually set email as confirmed
                    Address1 = "(Address)",
                    City = "(City)",
                    Country = "(Country)"
                };
                user.Roles = adminRole.ToUserRoles(user);
                result = await userHelper.AddUserAsync(user, adminPassword);
                if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
#if DEBUG
                if (await dataContext.Users.CountAsync() == 1) // only create test users if the admin user is the only one
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

            if (!await dataContext.Cities.AnyAsync())
            {
                await dataContext.Cities.AddRangeAsync([
                    new City {
                        Name = "Lisbon",
                        Country = "Portugal"
                    },
                    new City {
                        Name = "London",
                        Country = "UK"
                    },
                    new City {
                        Name = "Madrid",
                        Country = "Spain"
                    },
                    new City {
                        Name = "New York",
                        Country = "USA"
                    },
                    new City {
                        Name = "Paris",
                        Country = "France"
                    },
                    new City {
                        Name = "Sydney",
                        Country = "Australia"
                    },
                    new City {
                        Name = "Tokyo",
                        Country = "Japan"
                    }
                ]);
            }
            if (!await dataContext.AirplaneModels.AnyAsync())
            {
                await dataContext.AirplaneModels.AddAsync(new ModelAirplane
                {
                    ModelName = "Boeing 747",
                    PricePerHour = 1000,
                    MaxSeats = 660,
                    SeatRows = 55,
                    SeatColumns = 9,
                    WindowSeats = 3
                });
            }
            if (!await dataContext.Airplanes.AnyAsync())
            {
                await imageHelper.UploadAirlineImageAsync(Path.Combine(
                    Directory.GetCurrentDirectory() + @"wwwroot/images/easyJet.png"),
                    "347fbb37-0aa9-48a1-b3a9-84f82a774305");
                await dataContext.Airplanes.AddAsync(new Airplane
                {
                    Airline = "easyJet",
                    Description = "Boeing 747 - 495 seats",
                    AirlineImageId = "347fbb37-0aa9-48a1-b3a9-84f82a774305", // easyJet logo guid
                    AirplaneModelId = 1, // Assuming the first model is the Boeing 747
                    MaxSeats = 660,
                    SeatRows = 55,
                    SeatColumns = 9,
                    WindowSeats = 3
                });
            }
            await dataContext.SaveChangesAsync();
        }
#if DEBUG
        private readonly string testPassword = "12345Abc";
        public void CreateTestUsers(int amount, Roles role)
        {
            string roleName = role.ToString();
            var iRole = dataContext.Roles.FirstOrDefault(r => r.Name == roleName);
            if (iRole == null) throw new Exception($"Failed to create role '{roleName}'");
            string _email = $"test.{roleName.ToLowerInvariant()}{{0}}@yopmail.com"; // Using the yopmail service to access inbox
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
                    BirthDate = DateTime.UtcNow.AddYears(-20),
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
