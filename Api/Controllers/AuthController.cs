using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.DTOs.Auth;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    // =========================
    // LOGIN
    // =========================
    [AllowAnonymous]
    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelError());

        var result = await _authRepository.LoginAsync(request);

        return result.Success ? Ok(result) : Unauthorized(result);
    }

    // =========================
    // VERIFY OTP
    // =========================
    [AllowAnonymous]
    [HttpPost(nameof(VerifyOtp))]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelError());

        var result = await _authRepository.VerifyOtpAsync(request);

        return result.Success ? Ok(result) : Unauthorized(result);
    }

    // =========================
    // REFRESH TOKEN
    // =========================
    [AllowAnonymous]
    [HttpPost(nameof(RefreshToken))]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelError());

        var result = await _authRepository.RefreshTokenAsync(request);

        return result.Success ? Ok(result) : Unauthorized(result);
    }

    // =========================
    // LOGOUT
    // =========================
    [Authorize]
    [HttpPost(nameof(Logout))]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authRepository.LogoutAsync(request);
        return Ok(result);
    }

    // =========================
    // FORGOT PASSWORD
    // =========================
    [AllowAnonymous]
    [HttpPost(nameof(ForgotPassword))]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelError());

        var result = await _authRepository.ForgotPasswordAsync(request);

        return Ok(result); // siempre OK por seguridad (aunque el usuario no exista)
    }


    [AllowAnonymous]
    [HttpPost(nameof(ResetPassword))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelError());
        var result = await _authRepository.ResetPasswordAsync(request);

        return result.Success ? Ok(result) : BadRequest(result);
    }


    // =========================
    // CHANGE PASSWORD (usuario logueado)
    // =========================
    [Authorize]
    [HttpPost(nameof(ChangePassword))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelError());

        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var refreshToken =
            Request.Headers["X-Refresh-Token"].FirstOrDefault();

        var result = await _authRepository.ChangePasswordAsync(
            request,
            userId,
            refreshToken
        );

        return result.Success ? Ok(result) : BadRequest(result);
    }

    // =========================
    // SESIONES ACTIVAS
    // =========================
    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetMySessions()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        var result = await _authRepository.GetMySessionsAsync(userId);

        return Ok(result);
    }

    // =========================
    // REVOCAR SESIÓN
    // =========================
    [Authorize]
    [HttpPost("revoke-session/{id}")]
    public async Task<IActionResult> RevokeSession(int id)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var currentRefreshToken =
            Request.Headers["X-Refresh-Token"].FirstOrDefault();

        var result = await _authRepository.RevokeSessionAsync(
            id,
            userId,
            currentRefreshToken);

        return Ok(result);
    }

    // =========================
    // HELPERS
    // =========================
    private ApiOperationResultDto<object> ModelError()
    {
        return new ApiOperationResultDto<object>
        {
            Success = false,
            Code = StatusCodes.Status400BadRequest.ToString(),
            Message = "Los datos proporcionados no son válidos."
        };
    }

    [Authorize]
    [HttpPost("RevokeAllSessions")]
    public async Task<IActionResult> RevokeAllSessions()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var refreshToken =
            Request.Headers["X-Refresh-Token"].FirstOrDefault();

        var result = await _authRepository.RevokeAllSessionsAsync(
            userId,
            refreshToken
        );

        return result.Success ? Ok(result) : BadRequest(result);
    }
}