using Uam.LabHelpDesk.MvcClient.Models;
using Uam.LabHelpDesk.MvcClient.Models.Auth;

namespace Uam.LabHelpDesk.MvcClient.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponseModel<TokenResponseModel>> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}