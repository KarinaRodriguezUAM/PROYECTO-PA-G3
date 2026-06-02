namespace Uam.LabHelpDesk.Api.Interfaces;

/// <summary>
/// Contrato Unit of Work para coordinar repositorios y persistencia.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Repositorio de laboratorios.
    /// </summary>
    ILaboratoryRepository Laboratories { get; }

    /// <summary>
    /// Repositorio de equipos.
    /// </summary>
    IEquipmentRepository Equipment { get; }

    /// <summary>
    /// Guarda en base de datos todos los cambios pendientes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
