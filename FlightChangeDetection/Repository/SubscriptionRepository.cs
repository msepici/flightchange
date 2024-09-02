using FlightChangeDetection.Data;
using FlightChangeDetection.Interfaces;
using FlightChangeDetection.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FlightChangeDetection.Repository;

public class SubscriptionRepository(IServiceProvider serviceProvider) : ISubscriptionRepository
{
    public IEnumerable<Subscription> GetSubscriptions(int agencyId)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AirlineContext>();

            return context.Subscriptions
                .Where(s => s.AgencyId == agencyId)
                .ToList();
        }
    }
}
