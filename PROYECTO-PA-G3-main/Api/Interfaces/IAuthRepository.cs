using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.DTOs.Auth;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApiOperationResultDto<Uam.LabHelpDesk.Api.DTOs.Auth.LoginResponseDto>> LoginAsync(Uam.LabHelpDesk.Api.DTOs.Auth.LoginRequestDto request);
        Task<ApiOperationResultDto<AuthResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request);
        Task<ApiOperationResultDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<ApiOperationResultDto<bool>> LogoutAsync(RefreshTokenRequestDto request);
    }
}