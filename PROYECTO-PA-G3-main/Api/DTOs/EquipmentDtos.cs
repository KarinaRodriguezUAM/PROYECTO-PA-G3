using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.DTOs;

/// <summary>
/// DTO de salida para mostrar equipos en respuestas GET/POST/PUT.
/// </summary>
public record EquipmentDto(
    int Id,
    int LaboratoryId,
    string LaboratoryName,
    string Code,
    string Brand,
    string Model,
    string SerialNumber,
    string Type,
    string Status,
    DateOnly? PurchaseDate,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

/// <summary>
/// DTO de entrada para crear un equipo.
/// </summary>
public class CreateEquipmentDto
{
    /// <summary>
    /// ID del laboratorio al que pertenece el equipo.
    /// </summary>
    [Required]
    public int LaboratoryId { get; set; }

    /// <summary>
    /// Código único del equipo.
    /// </summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Marca del equipo.
    /// </summary>
    [Required, MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Modelo del equipo.
    /// </summary>
    [Required, MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Número de serie único del equipo.
    /// </summary>
    [Required, MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de equipo: Desktop, Laptop, Printer, Projector, Other.
    /// </summary>
    [Required, MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Estado del equipo: Operational, UnderRepair, Decommissioned.
    /// </summary>
    [Required, MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de compra del equipo (opcional).
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }
}

/// <summary>
/// DTO de entrada para actualizar un equipo existente.
/// </summary>
public class UpdateEquipmentDto
{
    /// <summary>
    /// ID del laboratorio al que pertenece el equipo.
    /// </summary>
    [Required]
    public int LaboratoryId { get; set; }

    /// <summary>
    /// Código único del equipo.
    /// </summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Marca del equipo.
    /// </summary>
    [Required, MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Modelo del equipo.
    /// </summary>
    [Required, MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Número de serie único del equipo.
    /// </summary>
    [Required, MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de equipo: Desktop, Laptop, Printer, Projector, Other.
    /// </summary>
    [Required, MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Estado del equipo: Operational, UnderRepair, Decommissioned.
    /// </summary>
    [Required, MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de compra del equipo (opcional).
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }
}
