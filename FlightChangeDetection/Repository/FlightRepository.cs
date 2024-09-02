using FlightChangeDetection.Data;
using FlightChangeDetection.Interfaces;
using FlightChangeDetection.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FlightChangeDetection.Repository;

public class FlightRepository(IServiceProvider serviceProvider) : IFlightRepository
{
    public IEnumerable<Flight> GetFlights(DateTime startDate, DateTime endDate, int batchSize, int batchIndex)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AirlineContext>();

            return context.Flights
                .Include(f => f.Route)
                .Where(f => f.DepartureTime >= startDate && f.DepartureTime <= endDate)
                .OrderBy(f => f.FlightId)
                .Skip(batchIndex * batchSize)
                .Take(batchSize)
                .AsEnumerable()
                .ToList();
        }
    }

    public int GetTotalFlights(DateTime startDate, DateTime endDate)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AirlineContext>();
            return context.Flights.Count(f => f.DepartureTime >= startDate && f.DepartureTime <= endDate);
        }
    }
}
