using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<AuthRepository> _localizer;

        public AuthRepository(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IStringLocalizer<AuthRepository> localizer)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _localizer = localizer;
        }

        public async Task<ApiOperationResultDto<DTOs.Auth.AuthResponseDto>> LoginAsync(DTOs.Auth.LoginRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiOperationResultDto<DTOs.Auth.AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["InvalidCredentials"]
                };
            }

            return await GenerateTokensAsync(user);
        }

        public async Task<ApiOperationResultDto<DTOs.Auth.AuthResponseDto>> RefreshTokenAsync(DTOs.Auth.RefreshTokenRequestDto request)
        {
            var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

            if (existingToken == null || !existingToken.IsActive)
            {
                return new ApiOperationResultDto<DTOs.Auth.AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["InvalidToken"]
                };
            }

            existingToken.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(existingToken);

            var user = await _unitOfWork.Users.GetByIdAsync(existingToken.UserId);

            if (user == null || !user.IsActive)
            {
                return new ApiOperationResultDto<DTOs.Auth.AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["InvalidUser"]
                };
            }

            return await GenerateTokensAsync(user);
        }

        public async Task<ApiOperationResultDto<bool>> LogoutAsync(DTOs.Auth.RefreshTokenRequestDto request)
        {
            var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

            if (existingToken != null)
            {
                existingToken.IsRevoked = true;
                _unitOfWork.RefreshTokens.Update(existingToken);
                await _unitOfWork.SaveChangesAsync();
            }

            return new ApiOperationResultDto<bool>
            {
                Success = true,
                Result = true,
                Message = _localizer["LogoutSuccess"]
            };
        }

        private async Task<ApiOperationResultDto<DTOs.Auth.AuthResponseDto>> GenerateTokensAsync(User user)
        {
            var roleResult = await _unitOfWork.Roles.GetRoleByIdAsync(user.RoleId);
            var roleName = roleResult.Result?.Name ?? string.Empty;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };

            var expirationMinutes = double.Parse(_configuration["Jwt:TokenExpirationMinutes"]!);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString("N");

            var expirationDays = double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!);
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(expirationDays),
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new ApiOperationResultDto<DTOs.Auth.AuthResponseDto>
            {
                Success = true,
                Result = new DTOs.Auth.AuthResponseDto(
                    accessToken,
                    refreshToken,
                    user.Email
                )
            };
        }
    }
}