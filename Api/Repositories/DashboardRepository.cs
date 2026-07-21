using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Repositories;

/// <summary>
/// Repositorio especializado para consultas agregadas y métricas del dashboard operativo.
/// </summary>
public class DashboardRepository(AppDbContext context, IStringLocalizer<DashboardRepository> localizer)
    : IDashboardRepository
{
    private readonly AppDbContext _context = context;
    private readonly IStringLocalizer<DashboardRepository> _localizer = localizer;

    /// <inheritdoc />
    public async Task<ApiOperationResultDto<GeneralSummaryDto>> GetGeneralSummaryAsync(CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<GeneralSummaryDto>();

        var totalReports = await _context.FaultReports.CountAsync(cancellationToken);
        var pendingCount = await _context.FaultReports.CountAsync(r => r.Status == "Pending", cancellationToken);
        var inProgressCount = await _context.FaultReports.CountAsync(r => r.Status == "InProgress", cancellationToken);
        var resolvedCount = await _context.FaultReports.CountAsync(r => r.Status == "Resolved", cancellationToken);
        var closedCount = await _context.FaultReports.CountAsync(r => r.Status == "Closed", cancellationToken);

        var totalEquipment = await _context.Equipment.CountAsync(e => e.IsActive, cancellationToken);
        var equipmentUnderRepair = await _context.Equipment.CountAsync(e => e.IsActive && e.Status == "UnderRepair", cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = _localizer["DashboardDataRetrievedSuccessfully"].Value;
        result.Result = new GeneralSummaryDto(
            TotalReports: totalReports,
            PendingCount: pendingCount,
            InProgressCount: inProgressCount,
            ResolvedCount: resolvedCount,
            ClosedCount: closedCount,
            TotalEquipment: totalEquipment,
            EquipmentUnderRepair: equipmentUnderRepair
        );

        return result;
    }

    /// <inheritdoc />
    public async Task<ApiOperationResultDto<List<ReportsByLabDto>>> GetReportsByLabAsync(CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<ReportsByLabDto>>();

        var reportsByLab = await _context.Laboratories
            .AsNoTracking()
            .Where(l => l.IsActive)
            .Select(l => new ReportsByLabDto(
                l.Id,
                l.Name,
                _context.FaultReports.Count(r => r.Equipment != null && r.Equipment.LaboratoryId == l.Id),
                _context.FaultReports.Count(r => r.Equipment != null && r.Equipment.LaboratoryId == l.Id && r.Status == "Pending"),
                _context.FaultReports.Count(r => r.Equipment != null && r.Equipment.LaboratoryId == l.Id && r.Status == "InProgress")
            ))
            .ToListAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = _localizer["DashboardDataRetrievedSuccessfully"].Value;
        result.Result = reportsByLab;

        return result;
    }

    /// <inheritdoc />
    public async Task<ApiOperationResultDto<List<ReportsByStatusDto>>> GetReportsByStatusAsync(CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<ReportsByStatusDto>>();

        var reportsByStatus = await _context.FaultReports
            .AsNoTracking()
            .GroupBy(r => r.Status)
            .Select(g => new ReportsByStatusDto(
                g.Key,
                g.Count()
            ))
            .ToListAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = _localizer["DashboardDataRetrievedSuccessfully"].Value;
        result.Result = reportsByStatus;

        return result;
    }

    /// <inheritdoc />
    public async Task<ApiOperationResultDto<List<ReportsByTechnicianDto>>> GetReportsByTechnicianAsync(CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<ReportsByTechnicianDto>>();

        var techRole = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == "Technician", cancellationToken);

        var techRoleId = techRole?.Id ?? 2;

        var techWorkload = await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive && u.RoleId == techRoleId)
            .Select(u => new ReportsByTechnicianDto(
                u.Id,
                u.FirstName + " " + u.LastName,
                _context.FaultReports.Count(r => r.AssignedToUserId == u.Id && r.Status == "InProgress"),
                _context.FaultReportStatusLogs.Count(l => l.ChangedByUserId == u.Id && l.NewStatus == "Resolved")
            ))
            .ToListAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = _localizer["DashboardDataRetrievedSuccessfully"].Value;
        result.Result = techWorkload;

        return result;
    }

    /// <inheritdoc />
    public async Task<ApiOperationResultDto<AverageResolutionTimeDto>> GetAverageResolutionTimeAsync(CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<AverageResolutionTimeDto>();

        var resolvedLogs = await _context.FaultReportStatusLogs
            .AsNoTracking()
            .Include(l => l.FaultReport)
            .Where(l => l.NewStatus == "Resolved" && l.FaultReport != null)
            .Select(l => new
            {
                ReportedAtUtc = l.FaultReport!.ReportedAtUtc,
                ResolvedAtUtc = l.ChangedAtUtc
            })
            .ToListAsync(cancellationToken);

        if (!resolvedLogs.Any())
        {
            result.Success = true;
            result.Code = StatusCodes.Status200OK.ToString();
            result.Message = _localizer["DashboardDataRetrievedSuccessfully"].Value;
            result.Result = new AverageResolutionTimeDto(
                AverageHours: 0,
                FastestResolutionHours: 0,
                SlowestResolutionHours: 0
            );
            return result;
        }

        var durationsInHours = resolvedLogs
            .Select(l => (l.ResolvedAtUtc - l.ReportedAtUtc).TotalHours)
            .Where(h => h >= 0)
            .ToList();

        if (!durationsInHours.Any())
        {
            result.Success = true;
            result.Code = StatusCodes.Status200OK.ToString();
            result.Message = _localizer["DashboardDataRetrievedSuccessfully"].Value;
            result.Result = new AverageResolutionTimeDto(
                AverageHours: 0,
                FastestResolutionHours: 0,
                SlowestResolutionHours: 0
            );
            return result;
        }

        var averageHours = Math.Round(durationsInHours.Average(), 2);
        var fastestHours = Math.Round(durationsInHours.Min(), 2);
        var slowestHours = Math.Round(durationsInHours.Max(), 2);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = _localizer["DashboardDataRetrievedSuccessfully"].Value;
        result.Result = new AverageResolutionTimeDto(
            AverageHours: averageHours,
            FastestResolutionHours: fastestHours,
            SlowestResolutionHours: slowestHours
        );

        return result;
    }
}
