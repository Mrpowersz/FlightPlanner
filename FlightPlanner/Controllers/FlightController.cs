using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        [HttpPost("search")]
        public ActionResult<IEnumerable<Flight>> Search([FromBody] SearchFlightsRequest request)
        {

            if (request == null)
            {
                return BadRequest("Invalid search criteria.");
            }

            if (request.From.Equals(request.To, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Departure and arrival airports cannot be the same.");
            }

            var flights = FlightStorage.SearchFlights(request);

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
            var flight = FlightStorage.GetFlightById(id);
            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }
    }
}
