using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Mvc;


namespace FlightPlanner.Controllers
{
    [Route("api/airports")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly IAirportService _airportService;
        private readonly IMapper _mapper;

        public AirportsController(IAirportService airportService, IMapper mapper)
        {
            _airportService = airportService;
            _mapper = mapper; 
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<AirportViewModel>> Search(string? search)
        {
            var airports = _airportService.SearchAirports(search);
            var airportViewModels = airports.Select(_mapper.Map<AirportViewModel>).ToList();
            return Ok(airportViewModels);
        }
    }
}



