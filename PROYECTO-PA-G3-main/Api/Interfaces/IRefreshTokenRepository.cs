using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        void Update(RefreshToken refreshToken);
    }
}