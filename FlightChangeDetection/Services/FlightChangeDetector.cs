using System.Collections.Concurrent;
using FlightChangeDetection.Interfaces;
using FlightChangeDetection.Models;

namespace FlightChangeDetection.Services;

public class FlightChangeDetector(IFlightRepository flightRepository, ISubscriptionRepository subscriptionRepository)
    : IFlightChangeDetector
{
    public IEnumerable<FlightChange> DetectChanges(DateTime startDate, DateTime endDate, int agencyId, int batchSize = 10000)
    {
        startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        var subscriptions = subscriptionRepository.GetSubscriptions(agencyId)
            .Select(s => new { s.OriginCityId, s.DestinationCityId })
            .ToList();

        if (!subscriptions.Any())
        {
            return new List<FlightChange>();
        }

        var allFlightChanges = new ConcurrentBag<FlightChange>();
        int totalFlights = flightRepository.GetTotalFlights(startDate, endDate);

        Parallel.For(0, (totalFlights + batchSize - 1) / batchSize, i =>
        {
            var filteredFlights = flightRepository.GetFlights(startDate, endDate, batchSize, i)
                .Where(f => subscriptions.Any(s =>
                    s.OriginCityId == f.Route.OriginCityId &&
                    s.DestinationCityId == f.Route.DestinationCityId))
                .ToList();

            var newFlightsQuery = from f in filteredFlights
                where !filteredFlights.Any(p =>
                    p.AirlineId == f.AirlineId &&
                    p.Route.OriginCityId == f.Route.OriginCityId &&
                    p.Route.DestinationCityId == f.Route.DestinationCityId &&
                    p.DepartureTime >= f.DepartureTime.AddDays(-7).ToUniversalTime().AddMinutes(-30) &&
                    p.DepartureTime <= f.DepartureTime.AddDays(-7).ToUniversalTime().AddMinutes(30))
                select new FlightChange { Flight = f, Status = "New" };

            var discontinuedFlightsQuery = from f in filteredFlights
                where !filteredFlights.Any(p =>
                    p.AirlineId == f.AirlineId &&
                    p.Route.OriginCityId == f.Route.OriginCityId &&
                    p.Route.DestinationCityId == f.Route.DestinationCityId &&
                    p.DepartureTime >= f.DepartureTime.AddDays(7).ToUniversalTime().AddMinutes(-30) &&
                    p.DepartureTime <= f.DepartureTime.AddDays(7).ToUniversalTime().AddMinutes(30))
                select new FlightChange { Flight = f, Status = "Discontinued" };

            foreach (var change in newFlightsQuery.Concat(discontinuedFlightsQuery))
            {
                allFlightChanges.Add(change);
            }
        });

        return allFlightChanges.ToList();
    }
}
