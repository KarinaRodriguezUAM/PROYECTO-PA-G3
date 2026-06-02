using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Repositories;

/// <summary>
/// Implementa Unit of Work para centralizar acceso a repositorios y guardado.
/// </summary>
public class UnitOfWork(
    AppDbContext context,
    IStringLocalizer<LaboratoryRepository> laboratoryLocalizer,
    IStringLocalizer<EquipmentRepository> equipmentLocalizer) : IUnitOfWork
{
    private ILaboratoryRepository? _laboratories;
    private IEquipmentRepository? _equipment;

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

    /// <summary>
    /// Guarda todos los cambios pendientes en base de datos.
    /// </summary>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
