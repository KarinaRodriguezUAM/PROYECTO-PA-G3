using Microsoft.EntityFrameworkCore;
using Uam.LabHelpDesk.Api.Data;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories;

public class PasswordResetRepository : IPasswordResetRepository
{
    private readonly AppDbContext _context;

    public PasswordResetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PasswordResetRequest request)
    {
        await _context.PasswordResetRequests.AddAsync(request);
    }

    public async Task<PasswordResetRequest?> GetActiveByUserIdAsync(int userId)
    {
        return await _context.PasswordResetRequests
            .Where(x =>
                x.UserId == userId &&
                !x.IsUsed &&
                x.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync();
    }

    public async Task<PasswordResetRequest?> GetBySessionTokenAsync(string sessionToken)
    {
        return await _context.PasswordResetRequests
            .FirstOrDefaultAsync(x => x.SessionToken == sessionToken);
    }

    public void Update(PasswordResetRequest request)
    {
        _context.PasswordResetRequests.Update(request);
    }
}