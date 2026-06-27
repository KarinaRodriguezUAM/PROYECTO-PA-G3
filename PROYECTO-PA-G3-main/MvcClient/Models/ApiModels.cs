namespace Uam.LabHelpDesk.MvcClient.Models;

/// <summary>
/// Modelo estándar para deserializar respuestas del API backend.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public T? Result { get; set; }
}

/// <summary>
/// Modelo para la respuesta del login JWT.
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
}

/// <summary>
/// DTO de lectura para laboratorios en el frontend MVC.
/// </summary>
public class LaboratoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

/// <summary>
/// DTO de entrada para crear o actualizar laboratorios desde el frontend MVC.
/// </summary>
public class LaboratoryUpsertDto
{
    public string Name { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; }
}

/// <summary>
/// DTO de lectura para equipos en el frontend MVC.
/// </summary>
public class EquipmentDto
{
    public int Id { get; set; }
    public int LaboratoryId { get; set; }
    public string LaboratoryName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PurchaseDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

/// <summary>
/// DTO de entrada para crear o actualizar equipos desde el frontend MVC.
/// </summary>
public class EquipmentUpsertDto
{
    public int LaboratoryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PurchaseDate { get; set; }
}
/// <summary>
/// DTO de lectura para Roles.
/// </summary>
public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

/// <summary>
/// DTO para crear o actualizar Roles.
/// </summary>
public class RoleUpsertDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// DTO de lectura para Users.
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO para crear o actualizar Users.
/// </summary>
public class UserUpsertDto
{
    public int RoleId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}