using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectAero96.Data.Entities;

namespace ProjectAero96.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<ModelAirplane> AirplaneModels { get; set; }
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightStop> FlightStops { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        // Faster than UserManager.Users.Count but less accurate (yet to confirm)
        public async Task<int> GetUserCountEstimateAsync()
        {
            return await Database
                .SqlQueryRaw<int>("SELECT SUM(row_count) AS rows FROM sys.dm_db_partition_stats WHERE object_id = OBJECT_ID('AspNetUsers') AND index_id < 2;")
                .FirstAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
