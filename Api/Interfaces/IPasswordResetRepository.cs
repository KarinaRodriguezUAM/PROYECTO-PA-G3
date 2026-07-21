using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces;

public interface IPasswordResetRepository
{
    Task AddAsync(PasswordResetRequest request);

    Task<PasswordResetRequest?> GetActiveByUserIdAsync(int userId);

    Task<PasswordResetRequest?> GetBySessionTokenAsync(string sessionToken);

    void Update(PasswordResetRequest request);
}