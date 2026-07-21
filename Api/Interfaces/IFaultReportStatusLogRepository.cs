using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces;

public interface IFaultReportStatusLogRepository : IRepository<FaultReportStatusLog>
{
    Task<ApiOperationResultDto<List<FaultReportStatusLogDto>>> GetLogsByFaultReportIdAsync(int faultReportId, CancellationToken cancellationToken = default);
}
