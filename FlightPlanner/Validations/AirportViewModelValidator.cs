﻿using FlightPlanner.Core.Models;
using FluentValidation;

namespace FlightPlanner.Validations
{
    public class AirportViewModelValidator : AbstractValidator<AirportViewModel>
    {
        public AirportViewModelValidator()
        {
            RuleFor(viewModel => viewModel.Airport).NotEmpty();
            RuleFor(viewModel => viewModel.City).NotEmpty();
            RuleFor(viewModel => viewModel.Country).NotEmpty();
        }
    }
}
