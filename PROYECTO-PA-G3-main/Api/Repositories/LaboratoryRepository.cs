using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

/// <summary>
/// Repositorio especializado con reglas de negocio del módulo de laboratorios.
/// </summary>
public class LaboratoryRepository(AppDbContext context, IStringLocalizer<LaboratoryRepository> localizer)
    : Repository<Laboratory>(context), ILaboratoryRepository
{
    /// <summary>
    /// Verifica si ya existe un laboratorio con el nombre dado, opcionalmente excluyendo un id.
    /// </summary>
    public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        return await Context.Laboratories.AnyAsync(
            x => x.Name.ToLower() == normalizedName && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    /// <summary>
    /// Obtiene la lista de laboratorios y la empaqueta en formato estándar de respuesta.
    /// </summary>
    public async Task<ApiOperationResultDto<List<LaboratoryDto>>> GetAllLaboratoriesAsync(CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<LaboratoryDto>>();
        var laboratories = await Context.Laboratories.AsNoTracking().ToListAsync(cancellationToken);
        var hasRecords = laboratories.Count > 0;

        result.Success = hasRecords;
        result.Code = hasRecords ? StatusCodes.Status200OK.ToString() : StatusCodes.Status404NotFound.ToString();
        result.Message = hasRecords ? localizer["OperationSuccessful"].Value : localizer["LaboratoriesNotFound"].Value;
        result.Result = hasRecords ? laboratories.Select(MapToDto).ToList() : null;

        return result;
    }

    /// <summary>
    /// Obtiene un laboratorio por id y lo empaqueta en formato estándar de respuesta.
    /// </summary>
    public async Task<ApiOperationResultDto<LaboratoryDto>> GetLaboratoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<LaboratoryDto>();
        var laboratory = await Context.Laboratories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        result.Success = laboratory is not null;
        result.Code = laboratory is not null ? StatusCodes.Status200OK.ToString() : StatusCodes.Status404NotFound.ToString();
        result.Message = laboratory is not null ? localizer["OperationSuccessful"].Value : localizer["LaboratoryNotFound"].Value;
        result.Result = laboratory is null ? null : MapToDto(laboratory);

        return result;
    }

    /// <summary>
    /// Crea un laboratorio validando duplicidad de nombre.
    /// </summary>
    public async Task<ApiOperationResultDto<LaboratoryDto>> CreateLaboratoryAsync(CreateLaboratoryDto resource, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<LaboratoryDto>();

        if (await NameExistsAsync(resource.Name, null, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["LaboratoryNameExists"].Value;
            return result;
        }

        var laboratory = new Laboratory
        {
            Name = resource.Name.Trim(),
            Building = resource.Building.Trim(),
            Floor = resource.Floor,
            Capacity = resource.Capacity,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await Context.Laboratories.AddAsync(laboratory, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status201Created.ToString();
        result.Message = localizer["LaboratoryCreatedSuccessfully"].Value;
        result.Result = MapToDto(laboratory);
        return result;
    }

    /// <summary>
    /// Actualiza un laboratorio validando existencia y duplicidad de nombre.
    /// </summary>
    public async Task<ApiOperationResultDto<LaboratoryDto>> UpdateLaboratoryAsync(int id, UpdateLaboratoryDto resource, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<LaboratoryDto>();
        var laboratory = await Context.Laboratories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (laboratory is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["LaboratoryNotFound"].Value;
            return result;
        }

        if (await NameExistsAsync(resource.Name, id, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["LaboratoryNameExists"].Value;
            return result;
        }

        laboratory.Name = resource.Name.Trim();
        laboratory.Building = resource.Building.Trim();
        laboratory.Floor = resource.Floor;
        laboratory.Capacity = resource.Capacity;
        laboratory.UpdatedAtUtc = DateTime.UtcNow;

        Context.Laboratories.Update(laboratory);
        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["LaboratoryUpdatedSuccessfully"].Value;
        result.Result = MapToDto(laboratory);
        return result;
    }

    /// <summary>
    /// Elimina lógicamente un laboratorio (IsActive = false).
    /// </summary>
    public async Task<ApiOperationResultDto<object>> DeleteLaboratoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<object>();
        var laboratory = await Context.Laboratories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (laboratory is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["LaboratoryNotFound"].Value;
            return result;
        }

        laboratory.IsActive = false;
        laboratory.UpdatedAtUtc = DateTime.UtcNow;

        Context.Laboratories.Update(laboratory);
        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["LaboratoryDeletedSuccessfully"].Value;
        return result;
    }

    /// <summary>
    /// Convierte la entidad Laboratory a su DTO de salida.
    /// </summary>
    private static LaboratoryDto MapToDto(Laboratory l) =>
        new(l.Id, l.Name, l.Building, l.Floor, l.Capacity, l.IsActive, l.CreatedAtUtc, l.UpdatedAtUtc);
}
