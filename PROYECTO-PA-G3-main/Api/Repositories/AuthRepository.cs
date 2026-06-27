using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.DTOs.Auth;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Models;

namespace Uam.LabHelpDesk.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<AuthRepository> _localizer;
        private readonly ISmtpService _smtpService;

        public AuthRepository(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IStringLocalizer<AuthRepository> localizer,
            ISmtpService smtpService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _localizer = localizer;
            _smtpService = smtpService;
        }

        public async Task<ApiOperationResultDto<Uam.LabHelpDesk.Api.DTOs.Auth.LoginResponseDto>> LoginAsync(Uam.LabHelpDesk.Api.DTOs.Auth.LoginRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiOperationResultDto<Uam.LabHelpDesk.Api.DTOs.Auth.LoginResponseDto>
                {
                    Success = false,
                    Message = _localizer["InvalidCredentials"]
                };
            }

            // Invalidate existing unused active OTPs for this user
            var activeOtps = await _unitOfWork.OtpCodes.GetActiveOtpCodesByUserIdAsync(user.Id);
            foreach (var oldOtp in activeOtps)
            {
                oldOtp.IsUsed = true;
                _unitOfWork.OtpCodes.Update(oldOtp);
            }

            // Generate secure 6-digit numeric OTP using RandomNumberGenerator
            string code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            // Generate server-side SessionToken (Guid)
            string sessionToken = Guid.NewGuid().ToString();

            // Expiration values from appsettings
            int otpExpirationMinutes = int.Parse(_configuration["OtpExpirationMinutes"] ?? "10");

            var otpEntity = new OtpCode
            {
                UserId = user.Id,
                Code = code,
                SessionToken = sessionToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(otpExpirationMinutes),
                IsUsed = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.OtpCodes.AddAsync(otpEntity);
            await _unitOfWork.SaveChangesAsync();

            // Send email
            string subject = "Código de verificación de seguridad - UAM Lab Help Desk";
            string body = $"<p>Su código de verificación OTP es: <strong>{code}</strong>. Este código vencerá en {otpExpirationMinutes} minutos.</p>";

            bool emailSent = await _smtpService.SendEmailAsync(user.Email, subject, body);
            if (!emailSent)
            {
                // Rollback / Remove the OTP code if sending fails
                _unitOfWork.OtpCodes.Remove(otpEntity);
                await _unitOfWork.SaveChangesAsync();

                return new ApiOperationResultDto<Uam.LabHelpDesk.Api.DTOs.Auth.LoginResponseDto>
                {
                    Success = false,
                    Message = _localizer["EmailSendFailed"]
                };
            }

            return new ApiOperationResultDto<Uam.LabHelpDesk.Api.DTOs.Auth.LoginResponseDto>
            {
                Success = true,
                Message = _localizer["OtpSent"] ?? "Código OTP enviado correctamente.",
                Result = new Uam.LabHelpDesk.Api.DTOs.Auth.LoginResponseDto(sessionToken)
            };
        }

        public async Task<ApiOperationResultDto<AuthResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            var otpEntity = await _unitOfWork.OtpCodes.GetBySessionTokenAsync(request.SessionToken);

            if (otpEntity == null)
            {
                return new ApiOperationResultDto<AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["InvalidSessionToken"]
                };
            }

            if (otpEntity.IsUsed)
            {
                return new ApiOperationResultDto<AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["UsedOtp"]
                };
            }

            if (otpEntity.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return new ApiOperationResultDto<AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["ExpiredOtp"]
                };
            }

            if (otpEntity.Code != request.Code)
            {
                return new ApiOperationResultDto<AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["InvalidOtp"]
                };
            }

            // Mark OTP as used (which also invalidates the session token)
            otpEntity.IsUsed = true;
            _unitOfWork.OtpCodes.Update(otpEntity);
            await _unitOfWork.SaveChangesAsync();

            // Return JWT and RefreshToken
            var user = otpEntity.User;
            if (user == null || !user.IsActive)
            {
                return new ApiOperationResultDto<AuthResponseDto>
                {
                    Success = false,
                    Message = _localizer["InvalidUser"]
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