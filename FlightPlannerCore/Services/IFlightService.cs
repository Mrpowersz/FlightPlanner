using FlightPlanner.Core.Models;

namespace FlightPlanner.Core.Services
{
    public interface IFlightService : IEntityService<Flight>
    {
        Flight? GetFullFlightById(int id);
        new void Create(Flight flight);
        bool DeleteFlight(int id);
        PageResult<Flight> SearchFlights(SearchFlightsRequest request);
        bool FlightExists(Flight flight);
        bool TryAddFlight(Flight flight);
    }
}
