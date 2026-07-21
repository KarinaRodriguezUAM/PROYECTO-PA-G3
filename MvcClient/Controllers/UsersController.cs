using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;

[Authorize]
public class UsersController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ApiClient");
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/Users/GetAllUsers", ct);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(ct);

        var apiResult = JsonSerializer.Deserialize<ApiResponse<List<UserDto>>>(content, JsonOptions);

        return Json(apiResult?.Result ?? new List<UserDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserUpsertDto dto, CancellationToken ct)
    {
        var response = await _client.PostAsJsonAsync("api/Users/CreateUser", dto, ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpsertDto dto, CancellationToken ct)
    {
        var response = await _client.PutAsJsonAsync($"api/Users/UpdateUser/{id}", dto, ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return StatusCode((int)response.StatusCode, content);
    }

    
    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var response = await _client.DeleteAsync($"api/Users/DeleteUser/{id}", ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return StatusCode((int)response.StatusCode, content);
    }
}