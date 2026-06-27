namespace Uam.LabHelpDesk.Api.DTOs.Auth
{
    public record AuthResponseDto(
        string AccessToken,
        string RefreshToken,
        string Email
    );
}