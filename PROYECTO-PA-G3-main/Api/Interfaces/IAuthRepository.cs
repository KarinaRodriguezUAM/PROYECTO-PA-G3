using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.DTOs.Auth;

namespace Uam.LabHelpDesk.Api.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApiOperationResultDto<LoginResponseDto>> LoginAsync(LoginRequestDto request);

        Task<ApiOperationResultDto<AuthResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request);

        Task<ApiOperationResultDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);

        Task<ApiOperationResultDto<bool>> LogoutAsync(RefreshTokenRequestDto request);

        // Password recovery
        Task<ApiOperationResultDto<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request);

        Task<ApiOperationResultDto<string>> ResetPasswordAsync(ResetPasswordRequestDto request);

        Task<ApiOperationResultDto<bool>> ChangePasswordAsync(
            ChangePasswordRequestDto request,
            int userId,
            string? currentRefreshToken = null);

        // Sessions
        Task<ApiOperationResultDto<List<SessionDto>>> GetMySessionsAsync(int userId);
        Task<ApiOperationResultDto<bool>> RevokeSessionAsync(int refreshTokenId,   int userId,string? currentRefreshToken);

        Task<ApiOperationResultDto<bool>> RevokeAllSessionsAsync(int userId, string? exceptToken = null);
    }
}
