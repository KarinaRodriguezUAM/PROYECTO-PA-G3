using Microsoft.EntityFrameworkCore;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            refreshToken.CreatedAtUtc = DateTime.UtcNow;
            await _context.Set<RefreshToken>().AddAsync(refreshToken);
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.Set<RefreshToken>().Update(refreshToken);
        }
    }
}