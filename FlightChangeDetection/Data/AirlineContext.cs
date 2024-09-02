// Data/AirlineContext.cs

using FlightChangeDetection.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightChangeDetection.Data;

public class AirlineContext(DbContextOptions<AirlineContext> options) : DbContext(options)
{
    public DbSet<Route> Routes { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>()
            .HasKey(s => new { s.AgencyId, s.OriginCityId, s.DestinationCityId });

        modelBuilder.Entity<Route>()
            .HasMany(r => r.Flights)
            .WithOne(f => f.Route)
            .HasForeignKey(f => f.RouteId);

        base.OnModelCreating(modelBuilder);
    }
}
