using FlightPlanner.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace FlightPlanner.Storage
{
    public class FlightStorage
    {
        public static List<Flight> _flights = new List<Flight>();
        private static int _id = 1;

        public static bool AddFlight(Flight flight)
        {
            lock(_flights)
            {
                bool flightExists = _flights.Any(f =>
                    f.Carrier == flight.Carrier &&
                    f.From.AirportCode == flight.From.AirportCode &&
                    f.To.AirportCode == flight.To.AirportCode &&
                    f.DepartureTime == flight.DepartureTime &&
                    f.ArrivalTime == flight.ArrivalTime);

                if (flightExists)
                {
                    return false;
                }
                flight.Id = _id++;
                _flights.Add(flight);

                return true;
            }
        }

        public static Flight? GetFlightById(int id)
        {
            return _flights.FirstOrDefault(flight => flight.Id == id);
        }

        public static void Clear()
        {
            _flights.Clear();
        }

        public static bool DeleteFlight(int id)
        {
            var flight = _flights.FirstOrDefault(f => f.Id == id);
            if (flight != null)
            {
                _flights.Remove(flight);
                return true; 
            }

            return false; 
        }

        public static List<Flight> SearchFlights(SearchFlightsRequest request)
        {
            return _flights.Where(f =>
                f.From.AirportCode.Equals(request.From, StringComparison.OrdinalIgnoreCase) &&
                f.To.AirportCode.Equals(request.To, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(f.DepartureTime).Date == request.DepartureDate.Date
            ).ToList();
        }
    }
}
