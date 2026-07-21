using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
            existingToken.RevokedAtUtc = DateTime.UtcNow;
            existingToken.RevokedReason = "Token refreshed";
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
        public async Task<ApiOperationResultDto<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            // 🔐 Respuesta SIEMPRE genérica (seguridad contra enumeración)
            var genericResponse = new ApiOperationResultDto<string>
            {
                Success = true,
                Result = null,
                Message = _localizer["ForgotPasswordEmailSent"]
                          ?? "Si el correo está registrado, recibirás instrucciones de recuperación."
            };

            // Si usuario no existe o está inactivo
            if (user == null || !user.IsActive)
            {
                return genericResponse;
            }

            // 1. Invalidar solicitudes anteriores activas
            var oldRequest = await _unitOfWork.PasswordResets.GetActiveByUserIdAsync(user.Id);

            if (oldRequest != null)
            {
                oldRequest.IsUsed = true;
                _unitOfWork.PasswordResets.Update(oldRequest);
            }

            // 2. Generar OTP
            string code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            // 3. SessionToken (IMPORTANTE: se necesita en MVC)
            string sessionToken = Guid.NewGuid().ToString();

            // 4. Expiración
            int expirationMinutes = int.Parse(
                _configuration["PasswordReset:CodeExpirationMinutes"] ?? "10"
            );

            // 5. Crear entidad
            var resetRequest = new PasswordResetRequest
            {
                UserId = user.Id,
                SessionToken = sessionToken,
                Code = code,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes),
                IsUsed = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.PasswordResets.AddAsync(resetRequest);
            await _unitOfWork.SaveChangesAsync();

            // 6. Enviar correo
            string subject = "Recuperación de contraseña - UAM Lab Help Desk";

            string body = $@"
        <p>Tu código de recuperación es:</p>
        <h2>{code}</h2>
        <p>Este código expira en {expirationMinutes} minutos.</p>
    ";

            bool emailSent =
                await _smtpService.SendEmailAsync(user.Email, subject, body);

            if (!emailSent)
            {
                resetRequest.IsUsed = true;
                _unitOfWork.PasswordResets.Update(resetRequest);
                await _unitOfWork.SaveChangesAsync();

                return new ApiOperationResultDto<string>
                {
                    Success = false,
                    Result = null,
                    Message = _localizer["EmailSendFailed"]
                              ?? "No se pudo enviar el correo de recuperación."
                };
            }

            return new ApiOperationResultDto<string>
            {
                Success = true,
                Result = sessionToken,
                Message = _localizer["ForgotPasswordEmailSent"]
            };
        }
        public async Task<ApiOperationResultDto<string>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var resetRequest = await _unitOfWork.PasswordResets
                .GetBySessionTokenAsync(request.SessionToken);

            if (resetRequest == null)
            {
                return new ApiOperationResultDto<string>
                {
                    Success = false,
                    Message = _localizer["InvalidSessionToken"]
                };
            }

            if (resetRequest.IsUsed)
            {
                return new ApiOperationResultDto<string>
                {
                    Success = false,
                    Message = _localizer["UsedResetCode"]
                };
            }

            if (resetRequest.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return new ApiOperationResultDto<string>
                {
                    Success = false,
                    Message = _localizer["ExpiredResetCode"]
                };
            }

            if (resetRequest.Code?.Trim() != request.Code?.Trim())
            {
                return new ApiOperationResultDto<string>
                {
                    Success = false,
                    Message = _localizer["InvalidResetCode"]
                };
            }

            var user = await _unitOfWork.Users.GetByIdAsync(resetRequest.UserId);

            if (user == null || !user.IsActive)
            {
                return new ApiOperationResultDto<string>
                {
                    Success = false,
                    Message = _localizer["InvalidUser"]
                };
            }

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
            {
                return new ApiOperationResultDto<string>
                {
                    Success = false,
                    Message = _localizer["NewPasswordMustBeDifferent"]
                };
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAtUtc = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);

            resetRequest.IsUsed = true;
            resetRequest.UsedAtUtc = DateTime.UtcNow;
            _unitOfWork.PasswordResets.Update(resetRequest);

            var sessions = await _unitOfWork.RefreshTokens.GetActiveByUserIdAsync(user.Id);

            foreach (var session in sessions)
            {
                session.IsRevoked = true;
                session.RevokedAtUtc = DateTime.UtcNow;
                session.RevokedReason = "Password reset";
                _unitOfWork.RefreshTokens.Update(session);
            }

            await _unitOfWork.SaveChangesAsync();

            return new ApiOperationResultDto<string>
            {
                Success = true,
                Result = "OK",
                Message = _localizer["PasswordResetSuccess"]
            };
        }

        public async Task<ApiOperationResultDto<bool>> ChangePasswordAsync(
    ChangePasswordRequestDto request,
    int userId,
    string? currentRefreshToken = null)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return new ApiOperationResultDto<bool>
                {
                    Success = false,
                    Message = _localizer["InvalidUser"]
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return new ApiOperationResultDto<bool>
                {
                    Success = false,
                    Message = _localizer["InvalidCurrentPassword"]
                };
            }

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
            {
                return new ApiOperationResultDto<bool>
                {
                    Success = false,
                    Message = _localizer["NewPasswordMustBeDifferent"]
                };
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);

            var sessions = await _unitOfWork.RefreshTokens.GetActiveByUserIdAsync(userId);

            foreach (var session in sessions)
            {
                if (!string.IsNullOrEmpty(currentRefreshToken) &&
                    session.Token == currentRefreshToken)
                    continue;

                session.IsRevoked = true;
                session.RevokedAtUtc = DateTime.UtcNow;
                session.RevokedReason = "Password changed";

                _unitOfWork.RefreshTokens.Update(session);
            }

            await _unitOfWork.SaveChangesAsync();

            return new ApiOperationResultDto<bool>
            {
                Success = true,
                Result = true,
                Message = _localizer["PasswordChangeSuccess"]
            };
        }

        public async Task<ApiOperationResultDto<List<SessionDto>>> GetMySessionsAsync(int userId)
        {
            var tokens = await _unitOfWork.RefreshTokens.GetActiveByUserIdAsync(userId);

            var sessions = tokens
                .Where(t => !t.IsRevoked && t.ExpiresAtUtc > DateTime.UtcNow)
                .Select(t => new SessionDto
                {
                    Id = t.Id,
                    CreatedAtUtc = t.CreatedAtUtc,
                    ExpiresAtUtc = t.ExpiresAtUtc
                })
                .ToList();

            return new ApiOperationResultDto<List<SessionDto>>
            {
                Success = true,
                Result = sessions
            };
        }

        public async Task<ApiOperationResultDto<bool>> RevokeSessionAsync(
            int refreshTokenId,
            int userId,
            string? currentRefreshToken)
        {
            var token = await _unitOfWork.RefreshTokens.GetByIdAsync(refreshTokenId);

            if (token == null || token.UserId != userId)
            {
                return new ApiOperationResultDto<bool>
                {
                    Success = false,
                    Message = _localizer["InvalidToken"]
                };
            }

            bool isCurrentSession = token.Token == currentRefreshToken;

            if (!token.IsRevoked)
            {
                token.IsRevoked = true;
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevokedReason = "Manual session revocation";

                _unitOfWork.RefreshTokens.Update(token);
                await _unitOfWork.SaveChangesAsync();
            }

            return new ApiOperationResultDto<bool>
            {
                Success = true,
                Result = isCurrentSession,
                Message = _localizer["LogoutSuccess"]
            };
        }

        public async Task<ApiOperationResultDto<bool>> RevokeAllSessionsAsync(int userId, string? exceptToken = null)
        {
            var sessions = await _unitOfWork.RefreshTokens.GetActiveByUserIdAsync(userId);

            if (sessions == null || !sessions.Any())
            {
                return new ApiOperationResultDto<bool>
                {
                    Success = true,
                    Result = true,
                    Message = _localizer["LogoutSuccess"]
                };
            }

            foreach (var session in sessions)
            {
                // Mantener la sesión actual (si viene definida)
                if (!string.IsNullOrEmpty(exceptToken) && session.Token == exceptToken)
                    continue;

                // Evitar reprocesar tokens ya revocados
                if (session.IsRevoked)
                    continue;

                session.IsRevoked = true;
                session.RevokedAtUtc = DateTime.UtcNow;
                session.RevokedReason = "Revoke all sessions";

                _unitOfWork.RefreshTokens.Update(session);
            }

            await _unitOfWork.SaveChangesAsync();

            return new ApiOperationResultDto<bool>
            {
                Success = true,
                Result = true,
                Message = _localizer["LogoutSuccess"]
            };
        }

    }
}