using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.DTOs;

/// <summary>
/// DTO de salida para mostrar laboratorios en respuestas GET/POST/PUT.
/// </summary>
public record LaboratoryDto(
    int Id,
    string Name,
    string Building,
    int Floor,
    int Capacity,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

/// <summary>
/// DTO de entrada para crear un laboratorio.
/// </summary>
public class CreateLaboratoryDto
{
    /// <summary>
    /// Nombre único del laboratorio.
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Edificio donde se ubica el laboratorio.
    /// </summary>
    [Required, MaxLength(50)]
    public string Building { get; set; } = string.Empty;

    /// <summary>
    /// Piso del edificio.
    /// </summary>
    [Required]
    public int Floor { get; set; }

    /// <summary>
    /// Capacidad del laboratorio (debe ser mayor a cero).
    /// </summary>
    [Required, Range(1, int.MaxValue)]
    public int Capacity { get; set; }
}

/// <summary>
/// DTO de entrada para actualizar un laboratorio existente.
/// </summary>
public class UpdateLaboratoryDto
{
    /// <summary>
    /// Nombre único del laboratorio.
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Edificio donde se ubica el laboratorio.
    /// </summary>
    [Required, MaxLength(50)]
    public string Building { get; set; } = string.Empty;

    /// <summary>
    /// Piso del edificio.
    /// </summary>
    [Required]
    public int Floor { get; set; }

    /// <summary>
    /// Capacidad del laboratorio (debe ser mayor a cero).
    /// </summary>
    [Required, Range(1, int.MaxValue)]
    public int Capacity { get; set; }
}
