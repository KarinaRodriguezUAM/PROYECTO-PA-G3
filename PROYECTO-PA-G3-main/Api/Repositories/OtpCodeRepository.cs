using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories
{
    /// <summary>
    /// Implementación de repositorio para gestionar persistencia de códigos OTP.
    /// </summary>
    public class OtpCodeRepository : Repository<OtpCode>, IOtpCodeRepository
    {
        public OtpCodeRepository(AppDbContext context) : base(context)
        {
        }

        /// <inheritdoc />
        public async Task<OtpCode?> GetBySessionTokenAsync(string sessionToken, CancellationToken cancellationToken = default)
        {
            return await Context.OtpCodes
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.SessionToken == sessionToken, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<OtpCode>> GetActiveOtpCodesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await Context.OtpCodes
                .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiresAtUtc > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }
    }
}
