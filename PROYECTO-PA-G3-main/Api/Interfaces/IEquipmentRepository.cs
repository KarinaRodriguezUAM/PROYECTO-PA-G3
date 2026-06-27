using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces;

/// <summary>
/// Contrato especializado del repositorio de equipos.
/// </summary>
public interface IEquipmentRepository : IRepository<Equipment>
{
    /// <summary>
    /// Verifica si ya existe un equipo con el código dado, opcionalmente excluyendo un id.
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si ya existe un equipo con el número de serie dado, opcionalmente excluyendo un id.
    /// </summary>
    Task<bool> SerialNumberExistsAsync(string serialNumber, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los equipos activos con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<List<EquipmentDto>>> GetAllEquipmentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un equipo por id con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<EquipmentDto>> GetEquipmentByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene los equipos de un laboratorio específico con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<List<EquipmentDto>>> GetEquipmentByLaboratoryAsync(int laboratoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un equipo con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<EquipmentDto>> CreateEquipmentAsync(CreateEquipmentDto resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un equipo con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<EquipmentDto>> UpdateEquipmentAsync(int id, UpdateEquipmentDto resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina lógicamente un equipo (IsActive = false) con formato estándar de respuesta API.
    /// </summary>
    Task<ApiOperationResultDto<object>> DeleteEquipmentAsync(int id, CancellationToken cancellationToken = default);
}
