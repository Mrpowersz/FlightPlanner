using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _lock = new();

        public AdminApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlight(int id)
        {
            var flight = _context.Flights
                .Include(flight => flight.To)
                .Include(flight => flight.From)
                .SingleOrDefault(flight => flight.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult AddFlight(Flight flight)
        {
            if (flight == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(flight.Carrier)
                || string.IsNullOrEmpty(flight.From.AirportCode)
                || string.IsNullOrEmpty(flight.To.AirportCode)
                || string.IsNullOrEmpty(flight.From.Country)
                || string.IsNullOrEmpty(flight.To.Country)
                || string.IsNullOrEmpty(flight.From.City)
                || string.IsNullOrEmpty(flight.To.City))
            {
                return BadRequest();
            }

            if (flight.From.AirportCode.Trim().Equals(flight.To.AirportCode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Departure and arrival airports cannot be the same.");
            }

            if (!DateTime.TryParse(flight.DepartureTime, out var departureTime) ||
                !DateTime.TryParse(flight.ArrivalTime, out var arrivalTime))
            {
                return BadRequest("Invalid date format for departure or arrival time.");
            }
            if (departureTime >= arrivalTime)
            {
                return BadRequest("Departure time must be before arrival time.");
            }

            lock (_lock)
            {
                var existingFlight = _context.Flights
                    .FirstOrDefault(f => f.Carrier == flight.Carrier
                                         && f.DepartureTime == flight.DepartureTime
                                         && f.ArrivalTime == flight.ArrivalTime
                                         && f.From.AirportCode == flight.From.AirportCode
                                         && f.To.AirportCode == flight.To.AirportCode
                                         && f.From.Country == flight.From.Country
                                         && f.To.Country == flight.To.Country
                                         && f.From.City == flight.From.City
                                         && f.To.City == flight.To.City);

                if (existingFlight != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Flight already exists.");
                }

                _context.Flights.Add(flight);
                _context.SaveChanges();
            }

            return Created($"api/flights/{flight.Id}", flight);
        }

        [HttpDelete("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
            }

            return Ok();
        }
    }
}
