using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.MvcClient.Models.Auth;

public class ResetPasswordViewModel
{
    [Required]
    public string SessionToken { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}