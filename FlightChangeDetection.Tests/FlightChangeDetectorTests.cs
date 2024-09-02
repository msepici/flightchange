using System;
using System.Collections.Generic;
using System.Linq;
using FlightChangeDetection.Interfaces;
using FlightChangeDetection.Models;
using FlightChangeDetection.Services;
using Moq;
using Xunit;

namespace FlightChangeDetection.Tests
{
    public class FlightChangeDetectorTests
    {
        private readonly Mock<IFlightRepository> _mockFlightRepository;
        private readonly Mock<ISubscriptionRepository> _mockSubscriptionRepository;
        private readonly FlightChangeDetector _flightChangeDetector;

        public FlightChangeDetectorTests()
        {
            _mockFlightRepository = new Mock<IFlightRepository>();
            _mockSubscriptionRepository = new Mock<ISubscriptionRepository>();

            _flightChangeDetector = new FlightChangeDetector(_mockFlightRepository.Object, _mockSubscriptionRepository.Object);
        }

       [Fact]
        public void DetectChanges_ShouldReturnNewFlight_WhenNoCorrespondingFlightInPastWeek()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date.AddHours(0); // Start of today
            var endDate = DateTime.UtcNow.Date.AddHours(23).AddMinutes(59); // End of today
            int batchSize = 1000;
            int batchIndex = 0;

            var newFlight = new Flight
            {
                FlightId = 1,
                Route = new Route { OriginCityId = 1, DestinationCityId = 2 },
                DepartureTime = DateTime.UtcNow.Date.AddHours(14), // 14:00 today
                ArrivalTime = DateTime.UtcNow.Date.AddHours(16).AddMinutes(55), // 16:55 today
                AirlineId = 1
            };

            // Simulate a past week's flight that is outside the 30-minute tolerance
            var pastWeekFlights = new List<Flight>
            {
                new Flight
                {
                    FlightId = 2,
                    Route = new Route { OriginCityId = 1, DestinationCityId = 2 },
                    DepartureTime = DateTime.UtcNow.Date.AddDays(-7).AddHours(12), // 12:00 seven days ago
                    ArrivalTime = DateTime.UtcNow.Date.AddDays(-7).AddHours(14).AddMinutes(55), // 14:55 seven days ago
                    AirlineId = 1
                }
            };

            _mockSubscriptionRepository.Setup(repo => repo.GetSubscriptions(It.IsAny<int>()))
                .Returns(new List<Subscription>
                {
                    new Subscription { OriginCityId = 1, DestinationCityId = 2 }
                });

            _mockFlightRepository.Setup(repo => repo.GetFlights(startDate.AddDays(-7), endDate, batchSize, batchIndex))
                .Returns(pastWeekFlights);

            _mockFlightRepository.Setup(repo => repo.GetFlights(startDate, endDate, batchSize, batchIndex))
                .Returns(new List<Flight> { newFlight });

            _mockFlightRepository.Setup(repo => repo.GetTotalFlights(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(2);

            // Act
            var result = _flightChangeDetector.DetectChanges(startDate, endDate, 1, batchSize);

            // Assert
            var flightChange = Assert.Single(result, fc => fc.Status == "New");
            Assert.Equal(newFlight.FlightId, flightChange.Flight.FlightId);
            Assert.Equal(newFlight.DepartureTime, flightChange.Flight.DepartureTime);
            Assert.Equal(newFlight.ArrivalTime, flightChange.Flight.ArrivalTime);
        }

        [Fact]
        public void DetectChanges_ShouldReturnDiscontinuedFlight_WhenNoCorrespondingFlightInNextWeek()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date.AddDays(-7).AddHours(0); // Start of 7 days ago
            var endDate = DateTime.UtcNow.Date.AddDays(-7).AddHours(23).AddMinutes(59); // End of 7 days ago
            int batchSize = 1000;
            int batchIndex = 0;

            var discontinuedFlight = new Flight
            {
                FlightId = 1,
                Route = new Route { OriginCityId = 1, DestinationCityId = 2 },
                DepartureTime = DateTime.UtcNow.Date.AddDays(-7).AddHours(14), // 14:00 seven days ago
                ArrivalTime = DateTime.UtcNow.Date.AddDays(-7).AddHours(16).AddMinutes(55), // 16:55 seven days ago
                AirlineId = 1
            };

            var nextWeekFlights = new List<Flight>
            {
                // A flight that does not match the discontinued flight time within the 7-day +/- 30 minutes tolerance
                new Flight
                {
                    FlightId = 2,
                    Route = new Route { OriginCityId = 1, DestinationCityId = 2 },
                    DepartureTime = DateTime.UtcNow.Date.AddHours(16), // 16:00 today (more than 30 minutes difference)
                    ArrivalTime = DateTime.UtcNow.Date.AddHours(18).AddMinutes(55), // 18:55 today
                    AirlineId = 1
                }
            };

            _mockSubscriptionRepository.Setup(repo => repo.GetSubscriptions(It.IsAny<int>()))
                .Returns(new List<Subscription>
                {
                    new Subscription { OriginCityId = 1, DestinationCityId = 2 }
                });

            // The past week's flight that we want to detect as discontinued
            _mockFlightRepository.Setup(repo => repo.GetFlights(startDate, endDate, batchSize, batchIndex))
                .Returns(new List<Flight> { discontinuedFlight });

            // The next week's flights that do not match the discontinued flight's time
            _mockFlightRepository.Setup(repo => repo.GetFlights(startDate.AddDays(7), endDate.AddDays(7), batchSize, batchIndex))
                .Returns(nextWeekFlights);

            _mockFlightRepository.Setup(repo => repo.GetTotalFlights(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(1);

            // Act
            var result = _flightChangeDetector.DetectChanges(startDate, endDate, 1, batchSize);

            // Assert
            var flightChange = Assert.Single(result, fc => fc.Status == "Discontinued");
            Assert.Equal(discontinuedFlight.FlightId, flightChange.Flight.FlightId);
            Assert.Equal(discontinuedFlight.DepartureTime, flightChange.Flight.DepartureTime);
            Assert.Equal(discontinuedFlight.ArrivalTime, flightChange.Flight.ArrivalTime);
        }
    }
}
