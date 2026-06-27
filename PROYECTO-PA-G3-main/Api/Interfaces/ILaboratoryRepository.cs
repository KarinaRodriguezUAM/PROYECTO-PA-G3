using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces;

/// <summary>
/// Contrato especializado del repositorio de laboratorios.
/// </summary>
public interface ILaboratoryRepository : IRepository<Laboratory>
{
    /// <summary>
    /// Verifica si ya existe un laboratorio con el nombre dado, opcionalmente excluyendo un id.
    /// </summary>
    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los laboratorios activos con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<List<LaboratoryDto>>> GetAllLaboratoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un laboratorio por id con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<LaboratoryDto>> GetLaboratoryByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un laboratorio con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<LaboratoryDto>> CreateLaboratoryAsync(CreateLaboratoryDto resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un laboratorio con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<LaboratoryDto>> UpdateLaboratoryAsync(int id, UpdateLaboratoryDto resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina lógicamente un laboratorio (IsActive = false) con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<object>> DeleteLaboratoryAsync(int id, CancellationToken cancellationToken = default);
}
