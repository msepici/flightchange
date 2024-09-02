using System.Diagnostics;
using FlightChangeDetection.Interfaces;
using FlightChangeDetection.Models;
using Microsoft.Extensions.Logging;

namespace FlightChangeDetection.Services;

public class FlightChangeDetectionService(
    IFlightChangeDetector flightChangeDetector,
    ILogger<FlightChangeDetectionService> logger)
{
    public void Run(string[] args)
        {
            if (args.Length < 3)
            {
                logger.LogError("Usage: AirlineApp.exe <start date> <end date> <agency id>");
                return;
            }

            if (!DateTime.TryParse(args[0], out DateTime startDate) ||
                !DateTime.TryParse(args[1], out DateTime endDate) ||
                !int.TryParse(args[2], out int agencyId))
            {
                logger.LogError("Invalid arguments. Ensure dates are in the correct format and agency id is an integer.");
                return;
            }

            try
            {
                logger.LogInformation("Starting flight change detection...");
                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();
                IEnumerable<FlightChange> changes = flightChangeDetector.DetectChanges(startDate, endDate, agencyId, 10000);
                sw.Stop();
                logger.LogInformation($"Flight change detection completed in {sw.ElapsedMilliseconds} ms.");
                OutputResults(changes);

                logger.LogInformation("Flight change detection completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during flight change detection.");
            }
        }

        private void OutputResults(IEnumerable<FlightChange> changes)
        {
            
            const string resultsFilePath = "results.csv";
            using (var writer = new StreamWriter(resultsFilePath))
            {
                writer.WriteLine("flight_id,origin_city_id,destination_city_id,departure_time,arrival_time,airline_id,status");

                foreach (var change in changes)
                {
                    var flight = change.Flight;
                    writer.WriteLine($"{flight.FlightId},{flight.Route.OriginCityId},{flight.Route.DestinationCityId},{flight.DepartureTime},{flight.ArrivalTime},{flight.AirlineId},{change.Status}");
                }
            }

            logger.LogInformation($"Results have been written to {resultsFilePath}");
        }
    }
