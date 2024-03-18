using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Services
{
    public class FlightService : EntityService<Flight>, IFlightService
    {
        private static readonly object _lock = new();
        public FlightService(IFlightPlannerDbContext context) : base(context)
        {
        }

        public Flight? GetFullFlightById(int id)
        {
            return _context.Flights
                .Include(flight => flight.From)
                .Include(flight => flight.To)
                .SingleOrDefault(flight => flight.Id == id);
        }

        public bool DeleteFlight(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
            }
            
            return true;
        }

        public new void Create(Flight flight)
        {
            _context.Flights.Add(flight);
            _context.SaveChanges();
        }

        public PageResult<Flight> SearchFlights(SearchFlightsRequest request)
        {
            var departureDateString = request.DepartureDate.ToString("yyyy-MM-dd");

            var query = _context.Flights
                .Include(flight => flight.From)
                .Include(flight => flight.To)
                .AsQueryable();

            query = query.Where(flight => flight.DepartureTime.StartsWith(departureDateString));

            if (!string.IsNullOrEmpty(request.From))
            {
                query = query.Where(flight => flight.From.AirportCode == request.From);
            }

            if (!string.IsNullOrEmpty(request.To))
            {
                query = query.Where(flight => flight.To.AirportCode == request.To);
            }

            var matches = query.ToList();

            return new PageResult<Flight>
            {
                Items = matches,
                TotalItems = matches.Count,
                Page = 0 
            };
        }

        public IEnumerable<Airport> SearchAirports(string search)
        {
            var query = _context.Airports.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();
                query = query.Where(airport => 
                                         airport.AirportCode.ToLower().Contains(normalizedSearch) ||
                                         airport.City.ToLower().Contains(normalizedSearch) ||
                                         airport.Country.ToLower().Contains(normalizedSearch));
            }

            return query.ToList();
        }

        public void ClearData()
        {
            _context.Flights.RemoveRange(_context.Flights);
            _context.Airports.RemoveRange(_context.Airports);
            _context.SaveChanges();
        }

        public bool FlightExists(Flight flight)
        {
            lock (_lock)
            {
                return _context.Flights
                    .Include(flight => flight.From)
                    .Include(flight => flight.To)
                    .Any(f =>
                        f.Carrier == flight.Carrier &&
                        f.DepartureTime == flight.DepartureTime &&
                        f.ArrivalTime == flight.ArrivalTime &&
                        f.From.AirportCode == flight.From.AirportCode &&
                        f.To.AirportCode == flight.To.AirportCode);
            }
        }

        public bool TryAddFlight(Flight flight)
        {
            lock (_lock)
            {
                var exists = FlightExists(flight);
                if (exists) return false;
                Create(flight);
                return true;
            }
        }
    }
}


