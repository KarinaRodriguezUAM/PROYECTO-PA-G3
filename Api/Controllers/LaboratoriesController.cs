using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Controllers;

/// <summary>
/// Controlador API para administrar operaciones CRUD de laboratorios.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LaboratoriesController(IUnitOfWork unitOfWork, IStringLocalizer<LaboratoriesController> stringLocalizer) : ControllerBase
{
    /// <summary>
    /// Obtiene la lista completa de laboratorios.
    /// </summary>
    [HttpGet(nameof(GetAllLaboratories))]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<LaboratoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllLaboratories(CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Laboratories.GetAllLaboratoriesAsync(cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Obtiene un laboratorio específico por su identificador.
    /// </summary>
    [HttpGet(nameof(GetLaboratoryById) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<LaboratoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLaboratoryById(int id, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Laboratories.GetLaboratoryByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Crea un nuevo laboratorio en la base de datos.
    /// </summary>
    [HttpPost(nameof(CreateLaboratory))]
    [ProducesResponseType(typeof(ApiOperationResultDto<LaboratoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLaboratory([FromBody] CreateLaboratoryDto resource, CancellationToken cancellationToken)
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

        var result = await unitOfWork.Laboratories.CreateLaboratoryAsync(resource, cancellationToken);
        return result.Success ? Created(string.Empty, result) : BadRequest(result);
    }

    /// <summary>
    /// Actualiza los datos de un laboratorio existente.
    /// </summary>
    [HttpPut(nameof(UpdateLaboratory) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<LaboratoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLaboratory(int id, [FromBody] UpdateLaboratoryDto resource, CancellationToken cancellationToken)
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

        var result = await unitOfWork.Laboratories.UpdateLaboratoryAsync(id, resource, cancellationToken);
        if (result.Success) return Ok(result);
        return result.Code == StatusCodes.Status404NotFound.ToString() ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Elimina lógicamente un laboratorio por su identificador.
    /// </summary>
    [HttpDelete(nameof(DeleteLaboratory) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLaboratory(int id, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Laboratories.DeleteLaboratoryAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
