namespace Uam.LabHelpDesk.Api.DTOs;

/// <summary>
/// Estructura estándar de respuesta para todos los endpoints.
/// </summary>
public class ApiOperationResultDto<T>
{
    /// <summary>
    /// Indica si la operación fue satisfactoria.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Código HTTP de la operación expresado como texto.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje descriptivo de la operación para la persona usuaria.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Datos devueltos por la operación. Puede ser nulo cuando no hay resultado.
    /// </summary>
    public T? Result { get; set; }
}
