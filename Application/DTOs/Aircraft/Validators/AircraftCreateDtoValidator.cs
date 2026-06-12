
using FluentValidation;

namespace AircraftMRO.Application.DTOs.Aircraft.Validators
{
    public class AircraftCreateDtoValidator : AbstractValidator<AircraftCreateDto>
{
    public AircraftCreateDtoValidator()
    {
        RuleFor(x => x.TailNumber)
            .NotEmpty()
            .WithMessage("Tail number is required");

        RuleFor(x => x.Model)
            .NotEmpty();

        RuleFor(x => x.Manufacturer)
            .NotEmpty();
    }
}
}