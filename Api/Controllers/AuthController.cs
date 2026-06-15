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

    [AllowAnonymous]
    [HttpPost(nameof(Login))]
    [ProducesResponseType(typeof(ApiOperationResultDto<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] Uam.LabHelpDesk.Api.DTOs.Auth.LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = "Los datos proporcionados no son válidos."
            });
        }

        var result = await _authRepository.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        result.Code = StatusCodes.Status200OK.ToString();

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost(nameof(RefreshToken))]
    [ProducesResponseType(typeof(ApiOperationResultDto<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] Uam.LabHelpDesk.Api.DTOs.Auth.RefreshTokenRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = "Los datos proporcionados no son válidos."
            });
        }

        var result = await _authRepository.RefreshTokenAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        result.Code = StatusCodes.Status200OK.ToString();

        return Ok(result);
    }

    [Authorize]
    [HttpPost(nameof(Logout))]
    [ProducesResponseType(typeof(ApiOperationResultDto<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] Uam.LabHelpDesk.Api.DTOs.Auth.RefreshTokenRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = "Los datos proporcionados no son válidos."
            });
        }

        var result = await _authRepository.LogoutAsync(request);

        result.Code = StatusCodes.Status200OK.ToString();

        return Ok(result);
    }
}