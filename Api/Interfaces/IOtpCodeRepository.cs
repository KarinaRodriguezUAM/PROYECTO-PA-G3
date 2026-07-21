using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    /// <summary>
    /// Contrato de repositorio para gestionar persistencia de códigos OTP.
    /// </summary>
    public interface IOtpCodeRepository : IRepository<OtpCode>
    {
        /// <summary>
        /// Obtiene un código OTP por su SessionToken.
        /// </summary>
        
        
        Task<OtpCode?> GetBySessionTokenAsync(string sessionToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todos los códigos OTP activos (no usados y no vencidos) para un usuario.
        /// </summary>
        Task<List<OtpCode>> GetActiveOtpCodesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}
