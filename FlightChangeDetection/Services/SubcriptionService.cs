using FlightChangeDetection.Data;
using FlightChangeDetection.Interfaces;
using FlightChangeDetection.Models;

namespace FlightChangeDetection.Services;

public class SubscriptionService(AirlineContext context) : ISubscriptionService
{
    public IEnumerable<Flight> FilterFlightsBySubscription(IEnumerable<Flight> flights, int agencyId)
    {
        var subscriptions = context.Subscriptions
            .Where(s => s.AgencyId == agencyId)
            .ToList();

        return flights.Where(f => subscriptions.Any(s =>
            s.OriginCityId == f.Route.OriginCityId && s.DestinationCityId == f.Route.DestinationCityId)).ToList();
    }
}
