using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;

[Authorize]

public class EquipmentController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ApiClient");
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/Equipment/GetAllEquipment", ct);
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(ct);
        var apiResult = JsonSerializer.Deserialize<ApiResponse<List<EquipmentDto>>>(content, JsonOptions);
        return Json(apiResult?.Result ?? new List<EquipmentDto>());
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveLaboratories(CancellationToken ct)
    {
        var response =
            await _client.GetAsync("api/Laboratories/GetAllLaboratories", ct);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(ct);

        var apiResult =
            JsonSerializer.Deserialize<ApiResponse<List<LaboratoryDto>>>(
                content,
                JsonOptions);

        var laboratories = apiResult?.Result?
    .Where(l => l.IsActive)
    .Select(l => new
    {
        id = l.Id,
        name = l.Name
    })
    .ToList();

        return Json(laboratories);
    }


    [HttpGet]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var response = await _client.GetAsync($"api/Equipment/GetEquipmentById/{id}", ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipmentUpsertDto dto, CancellationToken ct)
    {
        var response = await _client.PostAsJsonAsync("api/Equipment/CreateEquipment", dto, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentUpsertDto dto, CancellationToken ct)
    {
        var response = await _client.PutAsJsonAsync($"api/Equipment/UpdateEquipment/{id}", dto, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var response = await _client.DeleteAsync($"api/Equipment/DeleteEquipment/{id}", ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }
}