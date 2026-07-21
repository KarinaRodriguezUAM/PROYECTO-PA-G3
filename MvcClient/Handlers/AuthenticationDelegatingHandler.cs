using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Uam.LabHelpDesk.MvcClient.Models;
using Uam.LabHelpDesk.MvcClient.Models.Auth;

namespace Uam.LabHelpDesk.MvcClient.Handlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.User.Identity?.IsAuthenticated == true)
            {
                // Leer AccessToken de las cookies HttpOnly primero
                var token = context.Request.Cookies["AccessToken"];
                if (string.IsNullOrEmpty(token))
                {
                    token = context.User.FindFirst("AccessToken")?.Value;
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized && context != null && context.User.Identity?.IsAuthenticated == true)
            {
                // Leer RefreshToken de las cookies HttpOnly primero
                var refreshToken = context.Request.Cookies["RefreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    refreshToken = context.User.FindFirst("RefreshToken")?.Value;
                }

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var baseAddress = request.RequestUri?.GetLeftPart(UriPartial.Authority);
                    using var refreshClient = new HttpClient { BaseAddress = new Uri(baseAddress!) };

                    var refreshResponse = await refreshClient.PostAsJsonAsync("api/Auth/RefreshToken", new { RefreshToken = refreshToken }, cancellationToken);

                    if (refreshResponse.IsSuccessStatusCode)
                    {
                        var apiResponse = await refreshResponse.Content.ReadFromJsonAsync<ApiResponseModel<TokenResponseModel>>(cancellationToken: cancellationToken);
                        if (apiResponse != null && apiResponse.Success && apiResponse.Result != null)
                        {
                            // Actualizar cookies HttpOnly
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
                            var jsonToken = handler.ReadToken(apiResponse.Result.AccessToken) as JwtSecurityToken;

                            var claims = jsonToken?.Claims.ToList() ?? new List<Claim>();
                            claims.Add(new Claim("AccessToken", apiResponse.Result.AccessToken));
                            claims.Add(new Claim("RefreshToken", apiResponse.Result.RefreshToken));

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });

                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiResponse.Result.AccessToken);

                            response.Dispose();
                            response = await base.SendAsync(request, cancellationToken);
                        }
                    }
                }
            }

            return response;
        }
    }
}