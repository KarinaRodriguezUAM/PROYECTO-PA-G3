using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Uam.LabHelpDesk.MvcClient.Models;
using Uam.LabHelpDesk.MvcClient.Models.Auth;

namespace Uam.LabHelpDesk.MvcClient.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponseModel<LoginResponseModel>> InitiateLoginAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/Login",
                model);

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponseModel<LoginResponseModel>>();

            return apiResponse
                ?? new ApiResponseModel<LoginResponseModel>
                {
                    Success = false,
                    Message = "Error de comunicación con el servidor."
                };
        }

        public async Task<ApiResponseModel<TokenResponseModel>> VerifyOtpAsync(string sessionToken, string code)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/VerifyOtp",
                new { SessionToken = sessionToken, Code = code });

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponseModel<TokenResponseModel>>();

            if (apiResponse != null &&
                apiResponse.Success &&
                apiResponse.Result != null)
            {
                var context = _httpContextAccessor.HttpContext;

                if (context != null)
                {
                    // Almacenar AccessToken y RefreshToken en cookies HttpOnly
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    };
                    context.Response.Cookies.Append("AccessToken", apiResponse.Result.AccessToken, cookieOptions);
                    context.Response.Cookies.Append("RefreshToken", apiResponse.Result.RefreshToken, cookieOptions);

                    var handler = new JwtSecurityTokenHandler();

                    var jwt =
                        handler.ReadToken(apiResponse.Result.AccessToken)
                        as JwtSecurityToken;

                    var claims =
                        jwt?.Claims.ToList()
                        ?? new List<Claim>();

                    claims.Add(
                        new Claim(
                            ClaimTypes.Email,
                            apiResponse.Result.Email));

                    claims.Add(
                        new Claim(
                            "AccessToken",
                            apiResponse.Result.AccessToken));

                    claims.Add(
                        new Claim(
                            "RefreshToken",
                            apiResponse.Result.RefreshToken));

                    var identity =
                        new ClaimsIdentity(
                            claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal =
                        new ClaimsPrincipal(identity);

                    var authProperties =
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                        };

                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        authProperties);
                }
            }

            return apiResponse
                ?? new ApiResponseModel<TokenResponseModel>
                {
                    Success = false,
                    Message = "Error de comunicación con el servidor."
                };
        }

        public async Task LogoutAsync()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
                return;

            var refreshToken = context.Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                refreshToken = context.User.FindFirst("RefreshToken")?.Value;
            }

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _httpClient.PostAsJsonAsync(
                    "api/Auth/Logout",
                    new
                    {
                        RefreshToken = refreshToken
                    });
            }

            // Eliminar cookies HttpOnly de AccessToken y RefreshToken
            context.Response.Cookies.Delete("AccessToken");
            context.Response.Cookies.Delete("RefreshToken");

            await context.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}