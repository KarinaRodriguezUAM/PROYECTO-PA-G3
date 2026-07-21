using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.DTOs.Auth;

public class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}