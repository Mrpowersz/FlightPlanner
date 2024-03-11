using FlightPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("api/airports")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;

        public AirportsController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<Airport>> Search(string? search)
        {
            var flights = _context.Flights
                .Include(f => f.From) 
                .Include(f => f.To)
                .ToList();

            var airports = flights
                .SelectMany(flight => new[] { flight.From, flight.To })
                .DistinctBy(airport => airport.AirportCode)
                .ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();
                airports = airports
                    .Where(a => a.AirportCode.ToLower().Contains(normalizedSearch) ||
                                a.City.ToLower().Contains(normalizedSearch) ||
                                a.Country.ToLower().Contains(normalizedSearch))
                    .ToList();
            }

            return Ok(airports);
        }
    }
}



