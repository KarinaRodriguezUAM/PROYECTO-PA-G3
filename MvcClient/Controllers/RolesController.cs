using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;

[Authorize]

public class RolesController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ApiClient");
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/Roles/GetAllRoles", ct);
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(ct);
        var apiResult = JsonSerializer.Deserialize<ApiResponse<List<RoleDto>>>(content, JsonOptions);
        return Json(apiResult?.Result ?? new List<RoleDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleUpsertDto dto, CancellationToken ct)
    {
        var response = await _client.PostAsJsonAsync("api/Roles/CreateRole", dto, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var response = await _client.DeleteAsync($"api/Roles/DeleteRole/{id}", ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }
}