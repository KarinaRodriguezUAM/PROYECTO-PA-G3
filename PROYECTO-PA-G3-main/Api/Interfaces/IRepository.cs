namespace Uam.LabHelpDesk.Api.Interfaces;

/// <summary>
/// Contrato genérico para operaciones CRUD básicas sobre una entidad.
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Obtiene todos los registros de la entidad.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una entidad por su identificador.
    /// </summary>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva entidad al contexto.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca la entidad como modificada.
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Marca la entidad para eliminación.
    /// </summary>
    void Remove(TEntity entity);
}
