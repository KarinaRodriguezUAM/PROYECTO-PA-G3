using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);

        Task AddAsync(RefreshToken refreshToken);

        void Update(RefreshToken refreshToken);

        Task<List<RefreshToken>> GetActiveByUserIdAsync(int userId);

        Task<RefreshToken?> GetByIdAsync(int id);
    }
}