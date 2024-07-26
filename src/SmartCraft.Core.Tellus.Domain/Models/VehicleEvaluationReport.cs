namespace SmartCraft.Core.Tellus.Domain.Models;
public class VehicleEvaluationReport
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public required List<VehicleEvaluation> VehicleEvaluations { get; set; }
}
 