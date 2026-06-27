using System.ComponentModel.DataAnnotations;

namespace Uam.LabHelpDesk.Api.DTOs.Auth
{
    /// <summary>
    /// DTO de entrada para verificar el código OTP.
    /// </summary>
    public class VerifyOtpRequestDto
    {
        [Required(ErrorMessage = "El SessionToken es requerido.")]
        public string SessionToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código OTP es requerido.")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "El código OTP debe tener entre 6 y 10 caracteres.")]
        public string Code { get; set; } = string.Empty;
    }
}
