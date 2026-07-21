using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FaultReportsController(
    IUnitOfWork unitOfWork,
    IStringLocalizer<FaultReportsController> stringLocalizer) : ControllerBase
{


    [HttpGet(nameof(GetAllFaultReports))]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<FaultReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllFaultReports(CancellationToken cancellationToken)
    {
        var result = await unitOfWork.FaultReports.GetAllFaultReportsAsync(cancellationToken);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }


    [HttpGet(nameof(GetFaultReportById) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<FaultReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaultReportById(int id, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.FaultReports.GetFaultReportByIdAsync(id, cancellationToken);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }


    [HttpGet(nameof(GetFaultReportsByStatus) + "/{status}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<FaultReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaultReportsByStatus(string status, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.FaultReports.GetFaultReportsByStatusAsync(status, cancellationToken);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }

    [HttpGet(nameof(GetFaultReportsByEquipment) + "/{equipmentId:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<FaultReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaultReportsByEquipment(int equipmentId, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.FaultReports.GetFaultReportsByEquipmentAsync(equipmentId, cancellationToken);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }




    [HttpGet(nameof(GetFaultReportsByUser) + "/{userId:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<FaultReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaultReportsByUser(int userId, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.FaultReports.GetFaultReportsByUserAsync(userId, cancellationToken);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }

    [HttpPost(nameof(CreateFaultReport))]
    [ProducesResponseType(typeof(ApiOperationResultDto<FaultReportDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateFaultReport(
    [FromBody] CreateFaultReportDto resource,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = stringLocalizer["InvalidModel"].Value
            });
        }

        // Obtener el usuario autenticado desde el JWT
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            return Unauthorized(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = stringLocalizer["Unauthorized"].Value
            });
        }

        int userId = int.Parse(userIdClaim);

        // Consultar el usuario
        var userResult = await unitOfWork.Users.GetUserByIdAsync(userId, cancellationToken);

        if (!userResult.Success || userResult.Result is null)
        {
            return NotFound(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = stringLocalizer["UserNotFound"].Value
            });
        }

        // Regla 1: Solo Instructor puede crear reportes
        if (!userResult.Result.RoleName.Equals("Instructor", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = stringLocalizer["InstructorOnly"].Value
            });
        }

        var result = await unitOfWork.FaultReports.CreateFaultReportAsync(
            resource,
            userId,
            cancellationToken);

        return result.Success
            ? Created(string.Empty, result)
            : BadRequest(result);
    }

    /// <summary>
    /// Actualiza un reporte de avería.
    /// </summary>
    [HttpPut(nameof(UpdateFaultReport) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<FaultReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFaultReport(
        int id,
        [FromBody] UpdateFaultReportDto resource,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = stringLocalizer["InvalidModel"].Value
            });
        }

        var result = await unitOfWork.FaultReports.UpdateFaultReportAsync(
            id,
            resource,
            cancellationToken);

        if (result.Success)
            return Ok(result);

        return result.Code == StatusCodes.Status404NotFound.ToString()
            ? NotFound(result)
            : BadRequest(result);
    }


    /// <summary>
    /// Cierra un reporte de avería.
    /// </summary>
    [HttpPost(nameof(CloseFaultReport) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseFaultReport(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await unitOfWork.FaultReports.CloseFaultReportAsync(
            id,
            cancellationToken);

        if (result.Success)
            return Ok(result);

        return result.Code == StatusCodes.Status404NotFound.ToString()
            ? NotFound(result)
            : BadRequest(result);
    }

    [HttpGet("GetLogsByFaultReport/{faultReportId:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<FaultReportStatusLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLogsByFaultReport(int faultReportId, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.StatusLogs.GetLogsByFaultReportIdAsync(faultReportId, cancellationToken);

        return result.Success
            ? Ok(result)
            : NotFound(result);
    }

    [HttpPost("AssignFaultReport/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<FaultReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignFaultReport(int id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            return Unauthorized(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = stringLocalizer["Unauthorized"].Value
            });
        }

        int userId = int.Parse(userIdClaim);

        // Consultar el usuario para verificar el rol
        var userResult = await unitOfWork.Users.GetUserByIdAsync(userId, cancellationToken);

        if (!userResult.Success || userResult.Result is null)
        {
            return NotFound(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = stringLocalizer["UserNotFound"].Value
            });
        }

        if (!userResult.Result.RoleName.Equals("Technician", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = stringLocalizer["TechnicianOnly"].Value
            });
        }

        var result = await unitOfWork.FaultReports.AssignFaultReportAsync(id, userId, cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("UpdateFaultReportStatus/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<FaultReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFaultReportStatus(
        int id,
        [FromBody] UpdateFaultReportStatusDto resource,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = stringLocalizer["InvalidModel"].Value
            });
        }

        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            return Unauthorized(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = stringLocalizer["Unauthorized"].Value
            });
        }

        int userId = int.Parse(userIdClaim);

        // Consultar el usuario para verificar el rol
        var userResult = await unitOfWork.Users.GetUserByIdAsync(userId, cancellationToken);

        if (!userResult.Success || userResult.Result is null)
        {
            return NotFound(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = stringLocalizer["UserNotFound"].Value
            });
        }

        if (!userResult.Result.RoleName.Equals("Technician", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = stringLocalizer["TechnicianOnly"].Value
            });
        }

        var result = await unitOfWork.FaultReports.UpdateFaultReportStatusAsync(id, resource, userId, cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }
}

