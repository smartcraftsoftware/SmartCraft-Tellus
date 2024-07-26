using FluentValidation;
using SmartCraft.Core.Tellus.Domain.Models;

namespace SmartCraft.Core.Tellus.Domain.Validators;
public class VehicleEvaluationReportValidator : AbstractValidator<VehicleEvaluationReport>
{
    public VehicleEvaluationReportValidator()
    {
        RuleFor(x => x.StartTime).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);

        RuleFor(x => x.StopTime).NotEmpty().When(x => x.StopTime.HasValue)
            .LessThanOrEqualTo(DateTime.UtcNow).When(x => x.StopTime.HasValue)
            .GreaterThanOrEqualTo(x=> x.StartTime).When(x => x.StopTime.HasValue);
        
        RuleForEach(x => x.VehicleEvaluations).SetValidator(new VehicleEvaluationValidator()).When(x => x.VehicleEvaluations?.Count > 0);
    }
}

public class VehicleEvaluationValidator : AbstractValidator<VehicleEvaluation>
{
    public VehicleEvaluationValidator()
    {
        RuleFor(x => x.Vin)
            .NotEmpty().When(x => !string.IsNullOrEmpty(x.Vin));

        RuleFor(x => x.TotalEngineTime).GreaterThanOrEqualTo(0).When(x => x.TotalEngineTime.HasValue);

        RuleFor(x => x.AvgSpeed).GreaterThanOrEqualTo(0).When(x => x.AvgSpeed.HasValue);

        RuleFor(x => x.AvgFuelConsumption).GreaterThanOrEqualTo(0).When(x => x.AvgFuelConsumption.HasValue);

        RuleFor(x => x.AvgElectricEnergyConsumption).GreaterThanOrEqualTo(0).When(x => x.AvgElectricEnergyConsumption.HasValue);

        RuleFor(x => x.TotalFuelConsumption).GreaterThanOrEqualTo(0).When(x => x.TotalFuelConsumption.HasValue);

        RuleFor(x => x.FuelConsumptionPerHour).GreaterThanOrEqualTo(0).When(x => x.FuelConsumptionPerHour.HasValue);

        RuleFor(x => x.Co2Emissions).GreaterThanOrEqualTo(0).When(x => x.Co2Emissions.HasValue);

        RuleFor(x => x.Co2Saved).GreaterThanOrEqualTo(0).When(x => x.Co2Saved.HasValue);

        RuleFor(x => x.TotalDistance).GreaterThanOrEqualTo(0).When(x => x.TotalDistance.HasValue);

        RuleFor(x => x.TotalGasUsed).GreaterThanOrEqualTo(0).When(x => x.TotalGasUsed.HasValue);

        RuleFor(x => x.EngineRunningTime).Matches(@"^([0-1]?[0-9]|2[0-3]):([0-5]?[0-9]):([0-5]?[0-9])$").When(x => x.EngineRunningTime != null).Must(x => x == null || x.Length >= 1);
    }
}
