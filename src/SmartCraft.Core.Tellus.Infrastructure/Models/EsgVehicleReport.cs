namespace SmartCraft.Core.Tellus.Infrastructure.Models;
public class EsgVehicleReport : BaseDbModel
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public required List<VehicleEvaluation> VehicleEvaluations { get; set; }
}
