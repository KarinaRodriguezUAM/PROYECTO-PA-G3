using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.DTOs;

/// <summary>
/// DTO de salida para mostrar roles.
/// </summary>
public record RoleDto(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

/// <summary>
/// DTO de entrada para crear un rol.
/// </summary>
public class CreateRoleDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO de entrada para actualizar un rol.
/// </summary>
public class UpdateRoleDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}