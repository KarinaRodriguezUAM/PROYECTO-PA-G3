using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

/// <summary>
/// Repositorio especializado con reglas de negocio del módulo de roles.
/// </summary>
public class RoleRepository(
    AppDbContext context,
    IStringLocalizer<RoleRepository> localizer)
    : Repository<Role>(context), IRoleRepository
{
    public async Task<bool> NameExistsAsync(
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLowerInvariant();

        return await Context.Roles.AnyAsync(
            x => x.Name.ToLower() == normalizedName &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
    public async Task<ApiOperationResultDto<List<RoleDto>>> GetAllRolesAsync(
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<List<RoleDto>>();

        var roles = await Context.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var hasRecords = roles.Count > 0;

        result.Success = hasRecords;
        result.Code = hasRecords
            ? StatusCodes.Status200OK.ToString()
            : StatusCodes.Status404NotFound.ToString();

        result.Message = hasRecords
            ? localizer["OperationSuccessful"].Value
            : localizer["RolesNotFound"].Value;

        result.Result = hasRecords
            ? roles.Select(MapToDto).ToList()
            : null;

        return result;
    }

    public async Task<ApiOperationResultDto<RoleDto>> GetRoleByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<RoleDto>();

        var role = await Context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        result.Success = role is not null;

        result.Code = role is not null
            ? StatusCodes.Status200OK.ToString()
            : StatusCodes.Status404NotFound.ToString();

        result.Message = role is not null
            ? localizer["OperationSuccessful"].Value
            : localizer["RoleNotFound"].Value;

        result.Result = role is null
            ? null
            : MapToDto(role);

        return result;
    }

    public async Task<ApiOperationResultDto<RoleDto>> CreateRoleAsync(
    CreateRoleDto resource,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<RoleDto>();

        if (await NameExistsAsync(resource.Name, null, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["RoleNameExists"].Value;
            return result;
        }

        var role = new Role
        {
            Name = resource.Name.Trim(),
            Description = resource.Description,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await Context.Roles.AddAsync(role, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status201Created.ToString();
        result.Message = localizer["RoleCreatedSuccessfully"].Value;
        result.Result = MapToDto(role);

        return result;
    }

    public async Task<ApiOperationResultDto<RoleDto>> UpdateRoleAsync(
    int id,
    UpdateRoleDto resource,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<RoleDto>();

        var role = await Context.Roles
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (role is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["RoleNotFound"].Value;
            return result;
        }

        if (await NameExistsAsync(resource.Name, id, cancellationToken))
        {
            result.Success = false;
            result.Code = StatusCodes.Status400BadRequest.ToString();
            result.Message = localizer["RoleNameExists"].Value;
            return result;
        }

        role.Name = resource.Name.Trim();
        role.Description = resource.Description;
        role.UpdatedAtUtc = DateTime.UtcNow;

        Context.Roles.Update(role);

        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["RoleUpdatedSuccessfully"].Value;
        result.Result = MapToDto(role);

        return result;
    }
    public async Task<ApiOperationResultDto<object>> DeleteRoleAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var result = new ApiOperationResultDto<object>();

        var role = await Context.Roles
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (role is null)
        {
            result.Success = false;
            result.Code = StatusCodes.Status404NotFound.ToString();
            result.Message = localizer["RoleNotFound"].Value;
            return result;
        }

        role.IsActive = false;
        role.UpdatedAtUtc = DateTime.UtcNow;

        Context.Roles.Update(role);

        await Context.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Code = StatusCodes.Status200OK.ToString();
        result.Message = localizer["RoleDeletedSuccessfully"].Value;

        return result;
    }
    private static RoleDto MapToDto(Role role) =>
    new(
        role.Id,
        role.Name,
        role.Description,
        role.IsActive,
        role.CreatedAtUtc,
        role.UpdatedAtUtc
    );

}