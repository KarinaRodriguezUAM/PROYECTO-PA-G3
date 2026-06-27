using Uam.LabHelpDesk.MvcClient.Models;
using Uam.LabHelpDesk.MvcClient.Models.Auth;

namespace Uam.LabHelpDesk.MvcClient.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponseModel<LoginResponseModel>> InitiateLoginAsync(LoginViewModel model);
        Task<ApiResponseModel<TokenResponseModel>> VerifyOtpAsync(string sessionToken, string code);
        Task LogoutAsync();
    }
}