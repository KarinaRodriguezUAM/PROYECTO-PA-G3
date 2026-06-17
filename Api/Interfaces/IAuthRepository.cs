using Uam.LabHelpDesk.Api.DTOs;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApiOperationResultDto<DTOs.Auth.AuthResponseDto>> LoginAsync(DTOs.Auth.LoginRequestDto request);
        Task<ApiOperationResultDto<DTOs.Auth.AuthResponseDto>> RefreshTokenAsync(DTOs.Auth.RefreshTokenRequestDto request);
        Task<ApiOperationResultDto<bool>> LogoutAsync(DTOs.Auth.RefreshTokenRequestDto request);
    }
}