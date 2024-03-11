using FlightPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;

        public FlightController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpPost("search")]
        public ActionResult<PageResult<Flight>> Search([FromBody] SearchFlightsRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid search criteria.");
            }

            if (request.From.Equals(request.To, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Departure and arrival airports cannot be the same.");
            }

            var flights = _context.Flights
            .Include(f => f.From)
            .Include(f => f.To)
            .Where(f => f.From.AirportCode.ToLower() == request.From.ToLower() &&
                 f.To.AirportCode.ToLower() == request.To.ToLower() &&
                 f.DepartureTime.StartsWith(request.DepartureDate.ToString("yyyy-MM-dd")))
            .ToList();

            var result = new PageResult<Flight>
            {
                Page = flights.Any() ? 1 : 0,
                TotalItems = flights.Count,
                Items = flights
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<Flight> GetFlight(int id)
        {
            var flight = _context.Flights
                .Include(f => f.From)
                .Include(f => f.To)
                .FirstOrDefault(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }
    }
}
