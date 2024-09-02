using System.ComponentModel.DataAnnotations.Schema;

namespace FlightChangeDetection.Models;

public class Subscription
{
    [Column("agency_id")]
    public int AgencyId { get; set; }

    [Column("origin_city_id")]
    public int OriginCityId { get; set; }

    [Column("destination_city_id")]
    public int DestinationCityId { get; set; }
}
