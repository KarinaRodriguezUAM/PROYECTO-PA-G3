using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

/// <summary>
/// Repositorio especializado con reglas de negocio del módulo de equipos.
/// </summary>
public class EquipmentRepository(AppDbContext context, IStringLocalizer<EquipmentRepository> localizer)
    : Repository<Equipment>(context), IEquipmentRepository
{
    /// <summary>
    /// Verifica si ya existe un equipo con el código dado, opcionalmente excluyendo un id.
    /// </summary>
    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await Context.Equipment.AnyAsync(
            x => x.Code.ToUpper() == normalizedCode && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    /// <summary>
    /// Verifica si ya existe un equipo con el número de serie dado, opcionalmente excluyendo un id.
    /// </summary>
    public async Task<bool> SerialNumberExistsAsync(string serialNumber, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = serialNumber.Trim().ToUpperInvariant();
        return await Context.Equipment.AnyAsync(
            x => x.SerialNumber.ToUpper() == normalized && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    /// <summary>
    /// Obtiene la lista de equipos y la empaqueta en formato estándar de respuesta.
    /// </summary>
    public async Task<ApiOperationResultDto<List<EquipmentDto>>> GetAllEquipmentAsync(CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<EquipmentDto>>();
        var equipment = await Context.Equipment
            .AsNoTracking()
            .Include(e => e.Laboratory)
            .ToListAsync(cancellationToken);

        var hasRecords = equipment.Count > 0;
        result.Success = hasRecords;
        result.Code = hasRecords ? StatusCodes.Status200OK.ToString() : StatusCodes.Status404NotFound.ToString();
        result.Message = hasRecords ? localizer["OperationSuccessful"].Value : localizer["EquipmentNotFound"].Value;
        result.Result = hasRecords ? equipment.Select(MapToDto).ToList() : null;

        return result;
    }

    /// <summary>
    /// Obtiene un equipo por id y lo empaqueta en formato estándar de respuesta.
    /// </summary>
    public async Task<ApiOperationResultDto<EquipmentDto>> GetEquipmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<EquipmentDto>();
        var equipment = await Context.Equipment
            .AsNoTracking()
            .Include(e => e.Laboratory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        result.Success = equipment is not null;
        result.Code = equipment is not null ? StatusCodes.Status200OK.ToString() : StatusCodes.Status404NotFound.ToString();
        result.Message = equipment is not null ? localizer["OperationSuccessful"].Value : localizer["EquipmentItemNotFound"].Value;
        result.Result = equipment is null ? null : MapToDto(equipment);

        return result;
    }

    /// <summary>
    /// Obtiene los equipos de un laboratorio específico y los empaqueta en formato estándar de respuesta.
    /// </summary>
    public async Task<ApiOperationResultDto<List<EquipmentDto>>> GetEquipmentByLaboratoryAsync(int laboratoryId, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<EquipmentDto>>();

        var labExists = await Context.Laboratories.AnyAsync(x => x.Id == laboratoryId, cancellationToken);
        if (!labExists)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["LaboratoryNotFound"].Value;
            return result;
        }

        var equipment = await Context.Equipment
            .AsNoTracking()
            .Include(e => e.Laboratory)
            .Where(e => e.LaboratoryId == laboratoryId)
            .ToListAsync(cancellationToken);

        var hasRecords = equipment.Count > 0;
        result.Success = hasRecords;
        result.Code = hasRecords ? StatusCodes.Status200OK.ToString() : StatusCodes.Status404NotFound.ToString();
        result.Message = hasRecords ? localizer["OperationSuccessful"].Value : localizer["EquipmentNotFoundForLaboratory"].Value;
        result.Result = hasRecords ? equipment.Select(MapToDto).ToList() : null;

        return result;
    }

    /// <summary>
    /// Crea un equipo validando duplicidad de código y número de serie, y que el laboratorio esté activo.
    /// </summary>
    public async Task<ApiOperationResultDto<EquipmentDto>> CreateEquipmentAsync(CreateEquipmentDto resource, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<EquipmentDto>();

        var laboratory = await Context.Laboratories.FirstOrDefaultAsync(x => x.Id == resource.LaboratoryId, cancellationToken);
        if (laboratory is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["LaboratoryNotFound"].Value;
            return result;
        }

        if (!laboratory.IsActive)
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["LaboratoryInactive"].Value;
            return result;
        }

        if (await CodeExistsAsync(resource.Code, null, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["EquipmentCodeExists"].Value;
            return result;
        }

        if (await SerialNumberExistsAsync(resource.SerialNumber, null, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["EquipmentSerialNumberExists"].Value;
            return result;
        }

        var equipment = new Equipment
        {
            LaboratoryId = resource.LaboratoryId,
            Code = resource.Code.Trim().ToUpperInvariant(),
            Brand = resource.Brand.Trim(),
            Model = resource.Model.Trim(),
            SerialNumber = resource.SerialNumber.Trim().ToUpperInvariant(),
            Type = resource.Type.Trim(),
            Status = resource.Status.Trim(),
            PurchaseDate = resource.PurchaseDate,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await Context.Equipment.AddAsync(equipment, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        equipment.Laboratory = laboratory;

        result.Success = true;
        result.Code = StatusCodes.Status201Created.ToString();
        result.Message = localizer["EquipmentCreatedSuccessfully"].Value;
        result.Result = MapToDto(equipment);
        return result;
    }

    /// <summary>
    /// Actualiza un equipo validando existencia, duplicidad de código/serie y laboratorio activo.
    /// </summary>
    public async Task<ApiOperationResultDto<EquipmentDto>> UpdateEquipmentAsync(int id, UpdateEquipmentDto resource, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<EquipmentDto>();
        var equipment = await Context.Equipment
            .Include(e => e.Laboratory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (equipment is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["EquipmentItemNotFound"].Value;
            return result;
        }

        var laboratory = await Context.Laboratories.FirstOrDefaultAsync(x => x.Id == resource.LaboratoryId, cancellationToken);
        if (laboratory is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["LaboratoryNotFound"].Value;
            return result;
        }

        if (!laboratory.IsActive)
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["LaboratoryInactive"].Value;
            return result;
        }

        if (await CodeExistsAsync(resource.Code, id, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["EquipmentCodeExists"].Value;
            return result;
        }

        if (await SerialNumberExistsAsync(resource.SerialNumber, id, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["EquipmentSerialNumberExists"].Value;
            return result;
        }
        var hasOpenFaultReport = await Context.FaultReports.AnyAsync(
    x => x.EquipmentId == id &&
         x.Status != "Closed",
    cancellationToken);

        if (hasOpenFaultReport &&
            !string.Equals(resource.Status, "UnderRepair", StringComparison.OrdinalIgnoreCase))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = "No es posible cambiar el estado del equipo mientras exista un reporte de avería abierto.";

            return result;
        }



        equipment.LaboratoryId = resource.LaboratoryId;
        equipment.Code = resource.Code.Trim().ToUpperInvariant();
        equipment.Brand = resource.Brand.Trim();
        equipment.Model = resource.Model.Trim();
        equipment.SerialNumber = resource.SerialNumber.Trim().ToUpperInvariant();
        equipment.Type = resource.Type.Trim();
        equipment.Status = resource.Status.Trim();
        equipment.PurchaseDate = resource.PurchaseDate;
        equipment.UpdatedAtUtc = DateTime.UtcNow;
        equipment.Laboratory = laboratory;

        Context.Equipment.Update(equipment);
        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["EquipmentUpdatedSuccessfully"].Value;
        result.Result = MapToDto(equipment);
        return result;
    }

    /// <summary>
    /// Elimina lógicamente un equipo (IsActive = false).
    /// </summary>
    public async Task<ApiOperationResultDto<object>> DeleteEquipmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<object>();
        var equipment = await Context.Equipment.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (equipment is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["EquipmentItemNotFound"].Value;
            return result;
        }
        var hasOpenFaultReport = await Context.FaultReports.AnyAsync(
    x => x.EquipmentId == id &&
         x.Status != "Closed",
    cancellationToken);

        if (hasOpenFaultReport)
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = "No es posible eliminar un equipo que tiene un reporte de avería abierto.";

            return result;
        }


        equipment.IsActive = false;
        equipment.UpdatedAtUtc = DateTime.UtcNow;

        Context.Equipment.Update(equipment);
        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["EquipmentDeletedSuccessfully"].Value;
        return result;
    }

    /// <summary>
    /// Convierte la entidad Equipment a su DTO de salida.
    /// </summary>
    private static EquipmentDto MapToDto(Equipment e) =>
        new(e.Id, e.LaboratoryId, e.Laboratory?.Name ?? string.Empty,
            e.Code, e.Brand, e.Model, e.SerialNumber, e.Type, e.Status,
            e.PurchaseDate, e.IsActive, e.CreatedAtUtc, e.UpdatedAtUtc);
}
