using FlightChangeDetection.Models;

namespace FlightChangeDetection.Interfaces;

public interface IFlightRepository
{
    IEnumerable<Flight> GetFlights(DateTime startDate, DateTime endDate, int batchSize, int batchIndex);
    int GetTotalFlights(DateTime startDate, DateTime endDate);
}
