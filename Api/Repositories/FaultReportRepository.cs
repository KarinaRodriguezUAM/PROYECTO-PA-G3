using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

public class FaultReportRepository(
    AppDbContext context,
    IStringLocalizer<FaultReportRepository> localizer,
    IEmailNotificationService? emailNotificationService = null,
    ILogger<FaultReportRepository>? logger = null)
    : Repository<FaultReport>(context), IFaultReportRepository
{

    private static FaultReportDto MapToDto(FaultReport report)
    {
        return new FaultReportDto
        {
            Id = report.Id,
            EquipmentId = report.EquipmentId,
            EquipmentCode = report.Equipment?.Code ?? string.Empty,
            ReportedByUserId = report.ReportedByUserId,
            ReportedBy = report.ReportedByUser == null
                ? string.Empty
                : $"{report.ReportedByUser.FirstName} {report.ReportedByUser.LastName}",
            Title = report.Title,
            Description = report.Description,
            Priority = report.Priority,
            Status = report.Status,
            ReportedAtUtc = report.ReportedAtUtc,
            CreatedAtUtc = report.CreatedAtUtc,
            UpdatedAtUtc = report.UpdatedAtUtc,
            AssignedToUserId = report.AssignedToUserId,
            AssignedToUser = report.AssignedToUser == null
                ? null
                : $"{report.AssignedToUser.FirstName} {report.AssignedToUser.LastName}"
        };
    }

    public async Task<ApiOperationResultDto<List<FaultReportDto>>> GetAllFaultReportsAsync(
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<FaultReportDto>>();

        var reports = await Context.FaultReports
            .AsNoTracking()
            .Include(f => f.Equipment)
                .ThenInclude(e => e.Laboratory)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .ToListAsync(cancellationToken);

        result.Success = reports.Any();
        result.Code = result.Success
            ? StatusCodes.Status200OK.ToString()
            : StatusCodes.Status404NotFound.ToString();

        result.Message = result.Success
            ? localizer["OperationSuccessful"].Value
            : localizer["FaultReportsNotFound"].Value;

        result.Result = result.Success
            ? reports.Select(MapToDto).ToList()
            : null;

        return result;
    }

    public async Task<ApiOperationResultDto<FaultReportDto>> GetFaultReportByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<FaultReportDto>();

        var report = await Context.FaultReports
            .AsNoTracking()
            .Include(f => f.Equipment)
                .ThenInclude(e => e.Laboratory)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (report == null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["FaultReportNotFound"].Value;
            return result;
        }

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["OperationSuccessful"].Value;
        result.Result = MapToDto(report);

        return result;
    }

    public async Task<ApiOperationResultDto<List<FaultReportDto>>> GetFaultReportsByStatusAsync(
    string status,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<FaultReportDto>>();

        var reports = await Context.FaultReports
            .AsNoTracking()
            .Include(f => f.Equipment)
                .ThenInclude(e => e.Laboratory)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .Where(f => f.Status == status)
            .ToListAsync(cancellationToken);

        result.Success = reports.Any();
        result.Code = result.Success
            ? StatusCodes.Status200OK.ToString()
            : StatusCodes.Status404NotFound.ToString();

        result.Message = result.Success
            ? localizer["OperationSuccessful"].Value
            : localizer["FaultReportsNotFound"].Value;

        result.Result = result.Success
            ? reports.Select(MapToDto).ToList()
            : null;

        return result;
    }

    public async Task<ApiOperationResultDto<List<FaultReportDto>>> GetFaultReportsByEquipmentAsync(
     int equipmentId,
     CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<FaultReportDto>>();

        var reports = await Context.FaultReports
            .AsNoTracking()
            .Include(f => f.Equipment)
                .ThenInclude(e => e.Laboratory)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .Where(f => f.EquipmentId == equipmentId)
            .ToListAsync(cancellationToken);

        result.Success = reports.Any();
        result.Code = result.Success
            ? StatusCodes.Status200OK.ToString()
            : StatusCodes.Status404NotFound.ToString();

        result.Message = result.Success
            ? localizer["OperationSuccessful"].Value
            : localizer["FaultReportsNotFound"].Value;

        result.Result = result.Success
            ? reports.Select(MapToDto).ToList()
            : null;

        return result;
    }

    public async Task<ApiOperationResultDto<List<FaultReportDto>>> GetFaultReportsByUserAsync(
    int userId,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<FaultReportDto>>();

        var reports = await Context.FaultReports
            .AsNoTracking()
            .Include(f => f.Equipment)
                .ThenInclude(e => e.Laboratory)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .Where(f => f.ReportedByUserId == userId)
            .ToListAsync(cancellationToken);

        result.Success = reports.Any();
        result.Code = result.Success
            ? StatusCodes.Status200OK.ToString()
            : StatusCodes.Status404NotFound.ToString();

        result.Message = result.Success
            ? localizer["OperationSuccessful"].Value
            : localizer["FaultReportsNotFound"].Value;

        result.Result = result.Success
            ? reports.Select(MapToDto).ToList()
            : null;

        return result;
    }

    public async Task<ApiOperationResultDto<FaultReportDto>> CreateFaultReportAsync(
    CreateFaultReportDto resource,
    int reportedByUserId,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<FaultReportDto>();

        // Buscar equipo
        var equipment = await Context.Equipment
            .Include(e => e.Laboratory)
            .FirstOrDefaultAsync(e => e.Id == resource.EquipmentId, cancellationToken);
        

        if (equipment is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["EquipmentNotFound"].Value;
            return result;
        }
        // Regla 2: Solo se puede reportar un equipo Operational
        if (!equipment.Status.Equals("Operational", StringComparison.OrdinalIgnoreCase))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["EquipmentNotOperational"].Value;
            return result;
        }

        var utcNow = DateTime.UtcNow;

        var report = new FaultReport
        {
            EquipmentId = resource.EquipmentId,
            ReportedByUserId = reportedByUserId,
            Title = resource.Title.Trim(),
            Description = resource.Description.Trim(),
            Priority = resource.Priority.Trim(),
            Status = "Pending",
            ReportedAtUtc = utcNow,
            CreatedAtUtc = utcNow,
            UpdatedAtUtc = utcNow
        };

        await Context.FaultReports.AddAsync(report, cancellationToken);

        // Regla 3: El equipo pasa automáticamente a UnderRepair
        equipment.Status = "UnderRepair";
        equipment.UpdatedAtUtc = utcNow;

        await Context.SaveChangesAsync(cancellationToken);

        // Cargar relaciones para devolver el DTO
        report.Equipment = equipment;
        report.ReportedByUser = await Context.Users
            .FirstAsync(u => u.Id == reportedByUserId, cancellationToken);

        try
        {
            if (emailNotificationService is not null)
            {
                await emailNotificationService.SendReportCreatedAsync(report);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error enviando notificación por correo para reporte creado {ReportId}.", report.Id);
        }

        result.Success = true;
        result.Code = StatusCodes.Status201Created.ToString();
        result.Message = localizer["FaultReportCreatedSuccessfully"].Value;
        result.Result = MapToDto(report);

        return result;
    }
    public async Task<ApiOperationResultDto<FaultReportDto>> UpdateFaultReportAsync(
    int id,
    UpdateFaultReportDto resource,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<FaultReportDto>();

        var report = await Context.FaultReports
            .Include(f => f.Equipment)
                .ThenInclude(e => e.Laboratory)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (report is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["FaultReportNotFound"].Value;
            return result;
        }

        // Regla 7: un reporte cerrado no puede modificarse
        if (report.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["FaultReportAlreadyClosed"].Value;
            return result;
        }

        report.Title = resource.Title.Trim();
        report.Description = resource.Description.Trim();
        report.Priority = resource.Priority.Trim();
        report.Status = resource.Status.Trim();
        report.UpdatedAtUtc = DateTime.UtcNow;

        Context.FaultReports.Update(report);
        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["FaultReportUpdatedSuccessfully"].Value;
        result.Result = MapToDto(report);

        return result;
    }

    public async Task<ApiOperationResultDto<object>> CloseFaultReportAsync(
     int id,
     CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<object>();

        var report = await Context.FaultReports
            .Include(f => f.Equipment)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (report is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["FaultReportNotFound"].Value;
            return result;
        }

        // Regla 6: solo se puede cerrar si está Resolved
        if (!report.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["FaultReportMustBeResolved"].Value;
            return result;
        }

        report.Status = "Closed";
        report.UpdatedAtUtc = DateTime.UtcNow;

        // El equipo vuelve a estar operativo
        var equipment = await Context.Equipment
    .FirstOrDefaultAsync(e => e.Id == report.EquipmentId, cancellationToken);

        if (equipment is not null)
        {
            equipment.Status = "Operational";
            equipment.UpdatedAtUtc = DateTime.UtcNow;

            Context.Entry(equipment).State = EntityState.Modified;
        }
        
        await Context.SaveChangesAsync(cancellationToken);

        try
        {
            if (emailNotificationService is not null)
            {
                await emailNotificationService.SendReportClosedAsync(report);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error enviando notificación por correo para reporte cerrado {ReportId}.", report.Id);
        }

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["FaultReportClosedSuccessfully"].Value;

        return result;
    }

    public async Task<ApiOperationResultDto<FaultReportDto>> AssignFaultReportAsync(
        int id,
        int technicianUserId,
        CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<FaultReportDto>();

        var report = await Context.FaultReports
            .Include(f => f.Equipment)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (report is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["FaultReportNotFound"].Value;
            return result;
        }

        // Validar que el reporte esté en Pending
        if (!report.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["FaultReportNotPending"].Value;
            return result;
        }

        var previousStatus = report.Status;
        report.Status = "InProgress";
        report.AssignedToUserId = technicianUserId;
        report.UpdatedAtUtc = DateTime.UtcNow;

        var statusLog = new FaultReportStatusLog
        {
            FaultReportId = report.Id,
            ChangedByUserId = technicianUserId,
            PreviousStatus = previousStatus,
            NewStatus = report.Status,
            Notes = localizer["AutomaticAssignmentNotes"].Value,
            ChangedAtUtc = DateTime.UtcNow
        };

        await Context.FaultReportStatusLogs.AddAsync(statusLog, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        // Volver a cargar AssignedToUser para la respuesta DTO
        report.AssignedToUser = await Context.Users
            .FirstAsync(u => u.Id == technicianUserId, cancellationToken);

        try
        {
            if (emailNotificationService is not null && report.AssignedToUser is not null)
            {
                await emailNotificationService.SendReportAssignedAsync(report, report.AssignedToUser);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error enviando notificación por correo para asignación de reporte {ReportId}.", report.Id);
        }

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["FaultReportAssignedSuccessfully"].Value;
        result.Result = MapToDto(report);

        return result;
    }

    public async Task<ApiOperationResultDto<FaultReportDto>> UpdateFaultReportStatusAsync(
        int id,
        UpdateFaultReportStatusDto resource,
        int changedByUserId,
        CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<FaultReportDto>();

        var report = await Context.FaultReports
            .Include(f => f.Equipment)
            .Include(f => f.ReportedByUser)
            .Include(f => f.AssignedToUser)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (report is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["FaultReportNotFound"].Value;
            return result;
        }

        // Rechazar si el reporte está Closed
        if (report.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["FaultReportClosed"].Value;
            return result;
        }

        // Validar que solo el técnico asignado pueda cambiar el estado
        if (report.AssignedToUserId != changedByUserId)
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["NotAssignedTechnician"].Value;
            return result;
        }

        // Validar transición permitida (InProgress → Resolved es la única válida aquí)
        if (!report.Status.Equals("InProgress", StringComparison.OrdinalIgnoreCase))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["InvalidTransition"].Value;
            return result;
        }

        var previousStatus = report.Status;
        report.Status = "Resolved";
        report.UpdatedAtUtc = DateTime.UtcNow;

        var statusLog = new FaultReportStatusLog
        {
            FaultReportId = report.Id,
            ChangedByUserId = changedByUserId,
            PreviousStatus = previousStatus,
            NewStatus = report.Status,
            Notes = resource.Notes?.Trim(),
            ChangedAtUtc = DateTime.UtcNow
        };

        await Context.FaultReportStatusLogs.AddAsync(statusLog, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        try
        {
            if (emailNotificationService is not null)
            {
                await emailNotificationService.SendStatusChangedAsync(report, "Resolved");
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error enviando notificación por correo para estado Resolved del reporte {ReportId}.", report.Id);
        }

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["FaultReportUpdatedSuccessfully"].Value;
        result.Result = MapToDto(report);

        return result;
    }

}

    