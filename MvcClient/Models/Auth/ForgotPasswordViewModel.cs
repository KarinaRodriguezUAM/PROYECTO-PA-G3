using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.MvcClient.Models.Auth;

public class ForgotPasswordViewModel
{
    public string Email { get; set; } = string.Empty;
}