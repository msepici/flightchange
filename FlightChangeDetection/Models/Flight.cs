using System.ComponentModel.DataAnnotations.Schema;

namespace FlightChangeDetection.Models;

public class Flight
{
    [Column("flight_id")]
    public int FlightId { get; set; }

    [Column("route_id")]
    public int RouteId { get; set; }

    [Column("departure_time")]
    public DateTime DepartureTime
    {
        get => _departureTime;
        set => _departureTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    private DateTime _departureTime;

    [Column("arrival_time")]
    public DateTime ArrivalTime { get; set; }

    [Column("airline_id")]
    public int AirlineId { get; set; }

    public Route Route { get; set; }
}
