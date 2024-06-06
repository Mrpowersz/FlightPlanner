using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace FlightPlanner.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IMapper _mapper;

        public FlightController(IFlightService flightService, IMapper mapper)
        {
            _flightService = flightService;
            _mapper = mapper;
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

            var result = _flightService.SearchFlights(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<AddFlightResponse> GetFlight(int id)
        {
            var flight = _flightService.GetFullFlightById(id);
            if (flight == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AddFlightResponse>(flight));
        }
    }
}


