using Uam.LabHelpDesk.MvcClient.Models;
using Uam.LabHelpDesk.MvcClient.Models.Auth;

namespace Uam.LabHelpDesk.MvcClient.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponseModel<LoginResponseModel>> InitiateLoginAsync(LoginViewModel model);

        Task<ApiResponseModel<TokenResponseModel>> VerifyOtpAsync(string sessionToken, string code);

        Task LogoutAsync();

        Task<ApiResponseModel<string>> ForgotPasswordAsync(ForgotPasswordViewModel model);

        Task<ApiResponseModel<string>> ResetPasswordAsync(ResetPasswordViewModel model);

        Task<bool> ChangePasswordAsync(ChangePasswordViewModel model);

        Task<List<SessionViewModel>> GetMySessionsAsync();

        Task<bool> RevokeSessionAsync(int id);

        Task<bool> RevokeAllSessionsAsync();
    }
}

