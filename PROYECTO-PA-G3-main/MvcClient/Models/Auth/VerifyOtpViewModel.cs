using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.MvcClient.Models.Auth
{
    /// <summary>
    /// Modelo de vista para el formulario de verificación de OTP.
    /// </summary>
    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "El código OTP es requerido.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "El código OTP debe ser de exactamente 6 dígitos.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "El código OTP debe ser numérico.")]
        public string Code { get; set; } = string.Empty;
    }
}
