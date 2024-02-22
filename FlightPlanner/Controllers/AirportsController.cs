using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("api/airports")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        [HttpGet("")]
        public ActionResult<IEnumerable<Airport>> Search(string? search)
        {

            List<Airport> airports = new List<Airport>();

            if (FlightStorage._flights.Count > 0)
            {
                airports.Add(FlightStorage._flights[0].From);
            }


            foreach (Flight flight in FlightStorage._flights)
            {
                bool addFromAirport = true;
                bool addToAirport = true;
                foreach (var airport in airports)
                {
                    if (IsSameAirport(airport, flight.From) && addFromAirport)
                    {
                        addFromAirport = false;
                    }

                    if (IsSameAirport(airport, flight.To) && addToAirport)
                    {
                        addToAirport = false;
                    }
                }

                if (addFromAirport)
                {
                    airports.Add(flight.From);
                }

                if (addToAirport)
                {
                    airports.Add(flight.To);
                }
            }

            if (search == null)
            {
                return Ok(airports);
            }

            var normalizedSearch = search.Trim().ToLower();
            var matchedAirports = airports
                .Where(a => a.AirportCode.ToLower().Contains(normalizedSearch) ||
                            a.City.ToLower().Contains(normalizedSearch) ||
                            a.Country.ToLower().Contains(normalizedSearch))
                .ToList();

            return Ok(matchedAirports);
        }

        private static bool IsSameAirport(Airport left, Airport right)
        {
            return left.AirportCode == right.AirportCode && left.City == right.City &&
                   left.Country == right.Country;
        }
    }
}



