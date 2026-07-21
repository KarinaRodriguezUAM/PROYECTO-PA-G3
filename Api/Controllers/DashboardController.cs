using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Controllers;

/// <summary>
/// Controlador API para la consulta de métricas del Dashboard Operativo.
/// Acceso exclusivo para usuarios autenticados con rol Administrator.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class DashboardController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private bool IsAdministrator()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
        return string.Equals(roleClaim, "Administrator", StringComparison.OrdinalIgnoreCase) || User.IsInRole("Administrator");
    }

    [HttpGet("GeneralSummary")]
    public async Task<ActionResult<ApiOperationResultDto<GeneralSummaryDto>>> GetGeneralSummary(CancellationToken cancellationToken)
    {
        if (!IsAdministrator())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiOperationResultDto<GeneralSummaryDto>
            {
                Success = false,
                Code = StatusCodes.Status403Forbidden.ToString(),
                Message = "Acceso denegado. Se requiere rol Administrator."
            });
        }

        var result = await _unitOfWork.Dashboard.GetGeneralSummaryAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("ReportsByLab")]
    public async Task<ActionResult<ApiOperationResultDto<List<ReportsByLabDto>>>> GetReportsByLab(CancellationToken cancellationToken)
    {
        if (!IsAdministrator())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiOperationResultDto<List<ReportsByLabDto>>
            {
                Success = false,
                Code = StatusCodes.Status403Forbidden.ToString(),
                Message = "Acceso denegado. Se requiere rol Administrator."
            });
        }

        var result = await _unitOfWork.Dashboard.GetReportsByLabAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("ReportsByStatus")]
    public async Task<ActionResult<ApiOperationResultDto<List<ReportsByStatusDto>>>> GetReportsByStatus(CancellationToken cancellationToken)
    {
        if (!IsAdministrator())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiOperationResultDto<List<ReportsByStatusDto>>
            {
                Success = false,
                Code = StatusCodes.Status403Forbidden.ToString(),
                Message = "Acceso denegado. Se requiere rol Administrator."
            });
        }

        var result = await _unitOfWork.Dashboard.GetReportsByStatusAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("ReportsByTechnician")]
    public async Task<ActionResult<ApiOperationResultDto<List<ReportsByTechnicianDto>>>> GetReportsByTechnician(CancellationToken cancellationToken)
    {
        if (!IsAdministrator())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiOperationResultDto<List<ReportsByTechnicianDto>>
            {
                Success = false,
                Code = StatusCodes.Status403Forbidden.ToString(),
                Message = "Acceso denegado. Se requiere rol Administrator."
            });
        }

        var result = await _unitOfWork.Dashboard.GetReportsByTechnicianAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("AverageResolutionTime")]
    public async Task<ActionResult<ApiOperationResultDto<AverageResolutionTimeDto>>> GetAverageResolutionTime(CancellationToken cancellationToken)
    {
        if (!IsAdministrator())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiOperationResultDto<AverageResolutionTimeDto>
            {
                Success = false,
                Code = StatusCodes.Status403Forbidden.ToString(),
                Message = "Acceso denegado. Se requiere rol Administrator."
            });
        }

        var result = await _unitOfWork.Dashboard.GetAverageResolutionTimeAsync(cancellationToken);
        return Ok(result);
    }
}
