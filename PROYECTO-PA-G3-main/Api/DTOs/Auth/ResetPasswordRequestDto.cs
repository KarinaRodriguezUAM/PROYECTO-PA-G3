using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.DTOs.Auth;

public class ResetPasswordRequestDto
{
    [Required]
    public string SessionToken { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}