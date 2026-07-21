using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
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

        public async Task<ApiResponseModel<string>> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/resetpassword", model);

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponseModel<string>>();

            return apiResponse
                ?? new ApiResponseModel<string>
                {
                    Success = false,
                    Message = "Error de comunicación con el servidor."
                };
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var context = _httpContextAccessor.HttpContext;

            var token = context?.User.FindFirst("AccessToken")?.Value
                        ?? context?.Request.Cookies["AccessToken"];

            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/changepassword")
            {
                Content = JsonContent.Create(model)
            };

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<SessionViewModel>> GetMySessionsAsync()
        {
            var context = _httpContextAccessor.HttpContext;

            var token = context?.User.FindFirst("AccessToken")?.Value
                        ?? context?.Request.Cookies["AccessToken"];

            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/sessions");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<SessionViewModel>();

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponseModel<List<SessionViewModel>>>();

            if (apiResponse == null || !apiResponse.Success)
                return new List<SessionViewModel>();

            return apiResponse.Result ?? new List<SessionViewModel>();
        }

        public async Task<bool> RevokeSessionAsync(int id)
        {
            Console.WriteLine("=== REVOKE SESSION START ===");
            Console.WriteLine($"SessionId: {id}");

            var context = _httpContextAccessor.HttpContext;

            var token = context?.User.FindFirst("AccessToken")?.Value
                        ?? context?.Request.Cookies["AccessToken"];

            var refreshToken = context?.Request.Cookies["RefreshToken"];

            Console.WriteLine($"Has AccessToken: {!string.IsNullOrWhiteSpace(token)}");
            Console.WriteLine($"Has RefreshToken: {!string.IsNullOrWhiteSpace(refreshToken)}");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"api/auth/revoke-session/{id}");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                request.Headers.Add("X-Refresh-Token", refreshToken);
            }

            var response = await _httpClient.SendAsync(request);

            Console.WriteLine($"StatusCode: {response.StatusCode}");

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponseModel<bool>>();

            Console.WriteLine($"Success: {apiResponse?.Success}");
            Console.WriteLine($"IsCurrentSession: {apiResponse?.Result}");

            Console.WriteLine("=== REVOKE SESSION END ===");

            if (apiResponse == null || !apiResponse.Success)
                return false;

            // Ahora el bool indica si la sesión revocada era la actual
            return apiResponse.Result;
        }

        public async Task<bool> RevokeAllSessionsAsync()
        {
            var context = _httpContextAccessor.HttpContext;

            var accessToken =
                context?.User.FindFirst("AccessToken")?.Value
                ?? context?.Request.Cookies["AccessToken"];

            var refreshToken =
                context?.Request.Cookies["RefreshToken"];

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/auth/RevokeAllSessions");

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        accessToken);
            }

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                request.Headers.Add(
                    "X-Refresh-Token",
                    refreshToken);
            }

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public async Task<ApiResponseModel<string>> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/forgotpassword", model);

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponseModel<string>>();

            return apiResponse
                ?? new ApiResponseModel<string>
                {
                    Success = false,
                    Message = "Error de comunicación con el servidor."
                };
        }
    }
}