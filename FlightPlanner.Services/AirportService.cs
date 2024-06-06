using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;

namespace FlightPlanner.Services
{
    public class AirportService : DbService, IAirportService
    {
        public AirportService(IFlightPlannerDbContext context) : base(context)
        {
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
    }
}