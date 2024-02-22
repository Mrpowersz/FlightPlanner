using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlight(int id)
        {
            var flight = FlightStorage.GetFlightById(id);

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

            bool addedSuccessfully = FlightStorage.AddFlight(flight);
            if (!addedSuccessfully)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Flight already exists.");
            }

            return Created($"api/flights/{flight.Id}", flight);
        }

        [HttpDelete("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            FlightStorage.DeleteFlight(id);

            return Ok();
        }
    }
}
