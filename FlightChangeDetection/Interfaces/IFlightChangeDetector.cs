using FlightChangeDetection.Models;

namespace FlightChangeDetection.Interfaces;

public interface IFlightChangeDetector
{
    IEnumerable<FlightChange> DetectChanges(DateTime startDate, DateTime endDate, int agencyId, int batchSize = 10000);
}
