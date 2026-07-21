using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces;

public interface IFaultReportRepository : IRepository<FaultReport>
{
    Task<ApiOperationResultDto<List<FaultReportDto>>> GetAllFaultReportsAsync(CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<FaultReportDto>> GetFaultReportByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<List<FaultReportDto>>> GetFaultReportsByStatusAsync(string status, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<List<FaultReportDto>>> GetFaultReportsByEquipmentAsync(int equipmentId, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<List<FaultReportDto>>> GetFaultReportsByUserAsync(int userId, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<FaultReportDto>> CreateFaultReportAsync(CreateFaultReportDto resource, int reportedByUserId, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<FaultReportDto>> UpdateFaultReportAsync(int id, UpdateFaultReportDto resource, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<object>> CloseFaultReportAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<FaultReportDto>> AssignFaultReportAsync(int id, int technicianUserId, CancellationToken cancellationToken = default);

    Task<ApiOperationResultDto<FaultReportDto>> UpdateFaultReportStatusAsync(int id, UpdateFaultReportStatusDto resource, int changedByUserId, CancellationToken cancellationToken = default);
}