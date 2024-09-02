using FlightChangeDetection.Models;

namespace FlightChangeDetection.Interfaces;

public interface ISubscriptionRepository
{
    IEnumerable<Subscription> GetSubscriptions(int agencyId);
}
