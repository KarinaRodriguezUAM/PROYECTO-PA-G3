using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

/// <summary>
/// Implementa Unit of Work para centralizar acceso a repositorios y guardado.
/// </summary>
public class UnitOfWork(
    AppDbContext context,
    IStringLocalizer<LaboratoryRepository> laboratoryLocalizer,
    IStringLocalizer<EquipmentRepository> equipmentLocalizer,
    IStringLocalizer<RoleRepository> roleLocalizer,
    IStringLocalizer<UserRepository> userLocalizer) : IUnitOfWork
{
    private ILaboratoryRepository? _laboratories;
    private IEquipmentRepository? _equipment;
    private IRoleRepository? _roles;
    private IUserRepository? _users;

/// <summary>
/// Exposición pública del repositorio de laboratorios.
/// </summary>
public ILaboratoryRepository Laboratories =>
        _laboratories ??= new LaboratoryRepository(context, laboratoryLocalizer);

    /// <summary>
    /// Exposición pública del repositorio de equipos.
    /// </summary>
    public IEquipmentRepository Equipment =>
        _equipment ??= new EquipmentRepository(context, equipmentLocalizer);

public IRoleRepository Roles =>
    _roles ??= new RoleRepository(context, roleLocalizer);

public IUserRepository Users =>
    _users ??= new UserRepository(context, userLocalizer);

/// <summary>
/// 
/// 
/// 
/// 
/// Guarda todos los cambios pendientes en base de datos.
/// </summary>
public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
