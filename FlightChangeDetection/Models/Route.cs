using System.ComponentModel.DataAnnotations.Schema;

namespace FlightChangeDetection.Models;

public class Route
{
    [Column("route_id")]
    public int RouteId { get; set; }

    [Column("origin_city_id")]
    public int OriginCityId { get; set; }

    [Column("destination_city_id")]
    public int DestinationCityId { get; set; }

    [Column("departure_date")]
    public DateTime DepartureDate { get; set; }

    public ICollection<Flight> Flights { get; set; }
}
