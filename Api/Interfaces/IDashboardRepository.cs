using Uam.LabHelpDesk.Api.DTOs;

namespace Uam.LabHelpDesk.Api.Interfaces;

/// <summary>
/// Repositorio dedicado exclusivamente a las consultas de agregación y métricas del dashboard operativo.
/// </summary>
public interface IDashboardRepository
{
    /// <summary>
    /// Obtiene el resumen general con conteos de reportes por estado, equipos totales y equipos en reparación.
    /// </summary>
    Task<ApiOperationResultDto<GeneralSummaryDto>> GetGeneralSummaryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la distribución de reportes de averías agrupados por laboratorio.
    /// </summary>
    Task<ApiOperationResultDto<List<ReportsByLabDto>>> GetReportsByLabAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la distribución de reportes agrupados por estado.
    /// </summary>
    Task<ApiOperationResultDto<List<ReportsByStatusDto>>> GetReportsByStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la carga de trabajo asignada y resuelta agrupada por técnico de laboratorio.
    /// </summary>
    Task<ApiOperationResultDto<List<ReportsByTechnicianDto>>> GetReportsByTechnicianAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula el tiempo promedio, mínimo y máximo de resolución de averías (en horas).
    /// </summary>
    Task<ApiOperationResultDto<AverageResolutionTimeDto>> GetAverageResolutionTimeAsync(CancellationToken cancellationToken = default);
}
