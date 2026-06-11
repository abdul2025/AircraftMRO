using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AircraftMRO.Application.DTOs.Aircraft.Validators
{
    public class AircraftEditDtoValidator : AbstractValidator<AircraftEditDto>
    {
        public AircraftEditDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);


            RuleFor(x => x.Model)
                .NotEmpty();

            RuleFor(x => x.Manufacturer)
                .NotEmpty();
        }
    }
}