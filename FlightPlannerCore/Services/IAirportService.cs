using FlightPlanner.Core.Models;

namespace FlightPlanner.Core.Services
{
    public interface IAirportService
    {
        IEnumerable<Airport> SearchAirports(string search);
    }
}
