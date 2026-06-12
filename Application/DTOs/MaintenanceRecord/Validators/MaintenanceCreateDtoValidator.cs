using FluentValidation;
using AircraftMRO.Application.DTOs.MaintenanceRecord;

namespace AircraftMRO.Application.DTOs.MaintenanceRecord.Validators
{
    public class MaintenanceCreateDtoValidator : AbstractValidator<MaintenanceCreateDto>
    {
        public MaintenanceCreateDtoValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .GreaterThan(0)
                .WithMessage("Please select a valid Work Order.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Please select a valid Maintenance Type.");

            RuleFor(x => x.ScheduledDate)
                .NotEmpty()
                .WithMessage("Scheduled Date is required.");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}