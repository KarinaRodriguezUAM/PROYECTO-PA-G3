using System.Collections.Generic;

namespace Uam.LabHelpDesk.MvcClient.Models;

public record GeneralSummaryModel(
    int TotalReports,
    int PendingCount,
    int InProgressCount,
    int ResolvedCount,
    int ClosedCount,
    int TotalEquipment,
    int EquipmentUnderRepair
);

public record ReportsByLabModel(
    int LabId,
    string LabName,
    int TotalReports,
    int PendingCount,
    int InProgressCount
);

public record ReportsByTechnicianModel(
    int TechnicianId,
    string FullName,
    int AssignedCount,
    int ResolvedCount
);

public record ReportsByStatusModel(
    string Status,
    int Count
);

public record AverageResolutionTimeModel(
    double AverageHours,
    double FastestResolutionHours,
    double SlowestResolutionHours
);

public class DashboardViewModel
{
    public GeneralSummaryModel Summary { get; set; } = new(0, 0, 0, 0, 0, 0, 0);
    public List<ReportsByLabModel> ReportsByLab { get; set; } = new();
    public List<ReportsByTechnicianModel> ReportsByTechnician { get; set; } = new();
    public List<ReportsByStatusModel> ReportsByStatus { get; set; } = new();
    public AverageResolutionTimeModel ResolutionTime { get; set; } = new(0, 0, 0);
}
