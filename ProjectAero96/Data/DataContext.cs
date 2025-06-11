using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<
        User, IdentityRole, string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
        (options)
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<ModelAirplane> AirplaneModels { get; set; }
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightStop> FlightStops { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Explicitly configure UserRole relationships to avoid duplicate columns
            builder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

            // Set all FK delete behaviors to restrict
            var fks = builder.Model.GetEntityTypes()
                .Where(e => e is IEntity)
                .SelectMany(e => e.GetForeignKeys())
                .Where(fk => !fk.IsOwnership);
            foreach (var fk in fks)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
