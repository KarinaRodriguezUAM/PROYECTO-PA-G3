using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.MvcClient.Models.Auth;

public class ChangePasswordViewModel
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}