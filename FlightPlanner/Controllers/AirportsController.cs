using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Models;
using Microsoft.AspNetCore.Mvc;


namespace FlightPlanner.Controllers
{
    [Route("api/airports")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IMapper _mapper;

        public AirportsController(IFlightService flightService, IMapper mapper)
        {
            _flightService = flightService;
            _mapper = mapper; 
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<AirportViewModel>> Search(string? search)
        {
            var airports = _flightService.SearchAirports(search); 
            var airportViewModels = airports.Select(_mapper.Map<AirportViewModel>).ToList();
            return Ok(airportViewModels);
        }
    }
}



