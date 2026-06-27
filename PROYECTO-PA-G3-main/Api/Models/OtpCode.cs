using System;

namespace Uam.LabHelpDesk.Api.Models
{
    /// <summary>
    /// Entidad para almacenar los códigos OTP y sus tokens de sesión correspondientes.
    /// </summary>
    public class OtpCode
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public string Code { get; set; } = string.Empty;

        public string SessionToken { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
