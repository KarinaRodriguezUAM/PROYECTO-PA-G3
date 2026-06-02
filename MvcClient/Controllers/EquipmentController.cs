using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;

/// <summary>
/// Controlador MVC para el mantenimiento de equipos.
/// Funciona como cliente del API backend protegido con JWT.
/// </summary>
public class EquipmentController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:EquipmentBaseEndpoint"]}/GetAllEquipment";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        var apiResult = JsonSerializer.Deserialize<ApiResponse<List<EquipmentDto>>>(content, JsonOptions);
        return Json(apiResult?.Result ?? new List<EquipmentDto>());
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:EquipmentBaseEndpoint"]}/GetEquipmentById/{id}";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpGet]
    public async Task<IActionResult> GetByLaboratory(int laboratoryId, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:EquipmentBaseEndpoint"]}/GetEquipmentByLaboratory/{laboratoryId}";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        var apiResult = JsonSerializer.Deserialize<ApiResponse<List<EquipmentDto>>>(content, JsonOptions);
        return Json(apiResult?.Result ?? new List<EquipmentDto>());
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveLaboratories(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}/GetAllLaboratories";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        var apiResult = JsonSerializer.Deserialize<ApiResponse<List<LaboratoryDto>>>(content, JsonOptions);
        var active = (apiResult?.Result ?? new List<LaboratoryDto>()).Where(l => l.IsActive).ToList();
        return Json(active);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipmentUpsertDto dto, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:EquipmentBaseEndpoint"]}/CreateEquipment";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentUpsertDto dto, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:EquipmentBaseEndpoint"]}/UpdateEquipment/{id}";
        using var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:EquipmentBaseEndpoint"]}/DeleteEquipment/{id}";
        using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return StatusCode((int)response.StatusCode, content);
    }

    private async Task<string?> GetTokenAsync(HttpClient client, CancellationToken cancellationToken)
    {
        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:LoginEndpoint"]}";
        var payload = new
        {
            username = configuration["ApiSettings:Username"],
            password = configuration["ApiSettings:Password"]
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        using var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var apiResult = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(content, JsonOptions);
        return apiResult?.Result?.AccessToken;
    }
}
