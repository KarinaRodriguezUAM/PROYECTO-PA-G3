using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

public class FaultReportStatusLogRepository(
    AppDbContext context,
    IStringLocalizer<FaultReportStatusLogRepository> localizer)
    : Repository<FaultReportStatusLog>(context), IFaultReportStatusLogRepository
{
    public async Task<ApiOperationResultDto<List<FaultReportStatusLogDto>>> GetLogsByFaultReportIdAsync(
        int faultReportId,
        CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<FaultReportStatusLogDto>>();

        var logs = await Context.FaultReportStatusLogs
            .AsNoTracking()
            .Include(l => l.ChangedByUser)
            .Where(l => l.FaultReportId == faultReportId)
            .OrderByDescending(l => l.ChangedAtUtc)
            .ToListAsync(cancellationToken);

        result.Success = logs.Any();
        result.Code = result.Success
            ? Microsoft.AspNetCore.Http.StatusCodes.Status200OK.ToString()
            : Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound.ToString();

        result.Message = result.Success
            ? localizer["OperationSuccessful"].Value
            : localizer["LogsNotFound"].Value;

        result.Result = result.Success
            ? logs.Select(l => new FaultReportStatusLogDto(
                l.Id,
                l.FaultReportId,
                l.ChangedByUserId,
                $"{l.ChangedByUser.FirstName} {l.ChangedByUser.LastName}",
                l.PreviousStatus,
                l.NewStatus,
                l.Notes,
                l.ChangedAtUtc
            )).ToList()
            : null;

        return result;
    }
}
