using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IMapper _mapper;
        private readonly IValidator<AddFlightRequest> _validator;

        public AdminApiController(IFlightService flightService, IMapper mapper, IValidator<AddFlightRequest> validator)
        {
            _flightService = flightService;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlight(int id)
        {
            var flight = _flightService.GetFullFlightById(id);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AddFlightResponse>(flight));
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult AddFlight([FromBody] AddFlightRequest request)
        {
                if (request == null)
            {
                return BadRequest("Invalid request.");
            }
    
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            
                var flight = _mapper.Map<Flight>(request);

            var addedSuccessfully = _flightService.TryAddFlight(flight);
            if (!addedSuccessfully)
            {
                return Conflict("Flight already exists.");
            }

            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, _mapper.Map<AddFlightResponse>(flight));
        }

        [HttpDelete("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            _flightService.DeleteFlight(id);
            return Ok();
        }
    }
}
