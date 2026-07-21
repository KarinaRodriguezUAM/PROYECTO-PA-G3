namespace Uam.LabHelpDesk.Api.DTOs;

/// <summary>
/// DTO con el resumen general de conteos del parque tecnológico y averías.
/// </summary>
public record GeneralSummaryDto(
    int TotalReports,
    int PendingCount,
    int InProgressCount,
    int ResolvedCount,
    int ClosedCount,
    int TotalEquipment,
    int EquipmentUnderRepair
);

/// <summary>
/// DTO con la distribución de reportes de averías agrupados por laboratorio.
/// </summary>
public record ReportsByLabDto(
    int LabId,
    string LabName,
    int TotalReports,
    int PendingCount,
    int InProgressCount
);

/// <summary>
/// DTO con la carga de trabajo asignada y resuelta agrupada por técnico.
/// </summary>
public record ReportsByTechnicianDto(
    int TechnicianId,
    string FullName,
    int AssignedCount,
    int ResolvedCount
);

/// <summary>
/// DTO con conteos de reportes por estado.
/// </summary>
public record ReportsByStatusDto(
    string Status,
    int Count
);

/// <summary>
/// DTO con las métricas de tiempo promedio, mínimo y máximo de resolución de averías (en horas).
/// </summary>
public record AverageResolutionTimeDto(
    double AverageHours,
    double FastestResolutionHours,
    double SlowestResolutionHours
);
