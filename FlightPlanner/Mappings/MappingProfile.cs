using AutoMapper;
using FlightPlanner.Core.Models;

namespace FlightPlanner.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Airport, AirportViewModel>()
                .ForMember(viewModel => viewModel.Airport,
                options => options
                .MapFrom(source => source.AirportCode));
            CreateMap<AirportViewModel, Airport>()
                .ForMember(destination => destination.AirportCode,
                options => options
                .MapFrom(source => source.Airport));
            CreateMap<AddFlightRequest, Flight>()
                .ForMember(destination => destination.Id, options => options.Ignore());
            _ = CreateMap<Flight, AddFlightResponse>()
                .ForMember(destination => destination.From, action => action
                .MapFrom(source => source.From))
                .ForMember(destination => destination.To, action => action
                .MapFrom(source => source.To))
                .ForMember(destination => destination.Carrier, action => action
                .MapFrom(source => source.Carrier))
                .ForMember(destination => destination.DepartureTime, action => action
                .MapFrom(source => source.DepartureTime))
                .ForMember(destination => destination.ArrivalTime, action => action
                .MapFrom(source => source.ArrivalTime));
            CreateMap<Airport, AirportViewModel>()
                .ForMember(viewModel => viewModel.Airport, option => option
                .MapFrom(source => source.AirportCode));
        }
    }
}
