namespace Uam.LabHelpDesk.Api.DTOs.Auth
{
    /// <summary>
    /// DTO de salida para retornar el SessionToken temporal al iniciar sesión.
    /// </summary>
    public record LoginResponseDto(string SessionToken);
}
