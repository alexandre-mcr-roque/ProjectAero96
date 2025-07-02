using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using ProjectAero96.Data;
using ProjectAero96.Data.Entities;
using ProjectAero96.Data.Repositories;
using ProjectAero96.Helpers;
using ProjectAero96.Middleware;

namespace ProjectAero96
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Set required license keys
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["LicenseKeys:Syncfusion"]!);

            // Add services to the container.
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MainDB"));
            });

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;

                options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<DataContext>();

            // Repositories
            builder.Services.AddScoped<IFlightsRepository, FlightsRepository>()
                            .AddScoped<IAirplanesRepository, AirplanesRepository>()
                            .AddScoped<IAdminRepository, AdminRepository>();

            // Helpers
            builder.Services.AddScoped<IUserHelper, UserHelper>()
                            .AddScoped<IMailHelper, MailHelper>()
                            .AddScoped<IImageHelper, ImageHelper>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient<DbSeeder>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            // must be ran after populating HttpContext.User and before checking if they're have authorization for the action
            app.UseMiddleware<CheckUserDeletedMiddleware>();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            RunSeeder(app);
            app.Run();
        }

        private static void RunSeeder(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<DbSeeder>();
                seeder!.SeedAsync().Wait();
            }
        }
    }
}
