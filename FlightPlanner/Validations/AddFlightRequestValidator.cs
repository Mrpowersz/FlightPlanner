using FlightPlanner.Models;
using FluentValidation;

namespace FlightPlanner.Validations
{
    public class AddFlightRequestValidator : AbstractValidator<AddFlightRequest>
    {
        public AddFlightRequestValidator()
        {
            RuleFor(request => request.Carrier).NotEmpty();
            RuleFor(request => request.ArrivalTime)
                .NotEmpty()
                .Must(BeAValidDate)
                .WithMessage("ArrivalTime must be a valid date and time.");

            RuleFor(request => request.DepartureTime)
                .NotEmpty()
                .Must(BeAValidDate)
                .WithMessage("DepartureTime must be a valid date and time.")
                .Must((request, departureTime) =>
                    DateTime.Parse(departureTime) < DateTime.Parse(request.ArrivalTime))
                .WithMessage("Departure time must be before arrival time.")
                .When(request => BeAValidDate(request.DepartureTime) && BeAValidDate(request.ArrivalTime));

            RuleFor(request => request.To)
                .SetValidator(new AirportViewModelValidator());
            RuleFor(request => request.From)
                .SetValidator(new AirportViewModelValidator());

            RuleFor(request => request)
                .Must(request => !request.From.Airport.Trim().Equals(request.To.Airport.Trim(), StringComparison.OrdinalIgnoreCase))
                .WithMessage("Departure and arrival airports cannot be the same.");
        }

        private bool BeAValidDate(string value)
        {
            return DateTime.TryParse(value, out _);
        }
    }
}
