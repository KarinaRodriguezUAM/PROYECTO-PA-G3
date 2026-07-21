using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Uam.LabHelpDesk.Api.DTOs;
using Uam.LabHelpDesk.Api.Interfaces;

namespace Uam.LabHelpDesk.Api.Controllers;

/// <summary>
/// Controlador API para administrar operaciones CRUD de equipos.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EquipmentController(IUnitOfWork unitOfWork, IStringLocalizer<EquipmentController> stringLocalizer) : ControllerBase
{
    /// <summary>
    /// Obtiene la lista completa de equipos.
    /// </summary>
    [HttpGet(nameof(GetAllEquipment))]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<EquipmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllEquipment(CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Equipment.GetAllEquipmentAsync(cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Obtiene un equipo específico por su identificador.
    /// </summary>
    [HttpGet(nameof(GetEquipmentById) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<EquipmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEquipmentById(int id, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Equipment.GetEquipmentByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Obtiene todos los equipos de un laboratorio específico.
    /// </summary>
    [HttpGet(nameof(GetEquipmentByLaboratory) + "/{laboratoryId:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<List<EquipmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEquipmentByLaboratory(int laboratoryId, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Equipment.GetEquipmentByLaboratoryAsync(laboratoryId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Crea un nuevo equipo en la base de datos.
    /// </summary>
    [HttpPost(nameof(CreateEquipment))]
    [ProducesResponseType(typeof(ApiOperationResultDto<EquipmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentDto resource, CancellationToken cancellationToken)
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

        var result = await unitOfWork.Equipment.CreateEquipmentAsync(resource, cancellationToken);
        return result.Success ? Created(string.Empty, result) : BadRequest(result);
    }

    /// <summary>
    /// Actualiza los datos de un equipo existente.
    /// </summary>
    [HttpPut(nameof(UpdateEquipment) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<EquipmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEquipment(int id, [FromBody] UpdateEquipmentDto resource, CancellationToken cancellationToken)
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

        var result = await unitOfWork.Equipment.UpdateEquipmentAsync(id, resource, cancellationToken);
        if (result.Success) return Ok(result);
        return result.Code == StatusCodes.Status404NotFound.ToString() ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Elimina lógicamente un equipo por su identificador.
    /// </summary>
    [HttpDelete(nameof(DeleteEquipment) + "/{id:int}")]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEquipment(int id, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Equipment.DeleteEquipmentAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
