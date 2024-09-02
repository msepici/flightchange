using FlightChangeDetection.Models;

namespace FlightChangeDetection.Interfaces;

public interface ISubscriptionService
{
    IEnumerable<Flight> FilterFlightsBySubscription(IEnumerable<Flight> flights, int agencyId);
}
