namespace FlightChangeDetection.Models;

public class FlightChange
{
    public Flight Flight { get; set; }
    public string Status { get; set; } // "New" or "Discontinued"
}
