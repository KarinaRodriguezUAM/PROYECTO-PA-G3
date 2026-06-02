using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;


/// Controlador MVC para el mantenimiento de laboratorios.

public class LaboratoriesController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : Controller
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

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}/GetAllLaboratories";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        var apiResult = JsonSerializer.Deserialize<ApiResponse<List<LaboratoryDto>>>(content, JsonOptions);
        return Json(apiResult?.Result ?? new List<LaboratoryDto>());
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        var token = await GetTokenAsync(client, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { message = "No fue posible autenticar contra el API." });

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}/GetLaboratoryById/{id}";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LaboratoryUpsertDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var token = await GetTokenAsync(client, cancellationToken);
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { success = false, message = "No fue posible autenticar contra el API." });

            var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}/CreateLaboratory";
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = $"Error del API: {response.StatusCode}", details = content });

            var apiResult = JsonSerializer.Deserialize<ApiResponse<LaboratoryDto>>(content, JsonOptions);
            if (apiResult?.Success == true)
                return Json(new { success = true, message = "Laboratorio creado correctamente.", result = apiResult.Result });

            return Json(new { success = false, message = apiResult?.Message ?? "Error al crear el laboratorio" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Excepción: {ex.Message}", details = ex.InnerException?.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] LaboratoryUpsertDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var token = await GetTokenAsync(client, cancellationToken);
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { success = false, message = "No fue posible autenticar contra el API." });

            var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}/UpdateLaboratory/{id}";
            using var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = $"Error del API: {response.StatusCode}", details = content });

            var apiResult = JsonSerializer.Deserialize<ApiResponse<LaboratoryDto>>(content, JsonOptions);
            if (apiResult?.Success == true)
                return Json(new { success = true, message = "Laboratorio actualizado correctamente.", result = apiResult.Result });

            return Json(new { success = false, message = apiResult?.Message ?? "Error al actualizar el laboratorio" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Excepción: {ex.Message}", details = ex.InnerException?.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var token = await GetTokenAsync(client, cancellationToken);
            if (string.IsNullOrWhiteSpace(token))
                return Json(new { success = false, message = "No fue posible autenticar contra el API." });

            var endpoint = $"{configuration["ApiSettings:BaseUrl"]}{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}/DeleteLaboratory/{id}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = $"Error del API: {response.StatusCode}", details = content });

            var apiResult = JsonSerializer.Deserialize<ApiResponse<object>>(content, JsonOptions);
            if (apiResult?.Success == true)
                return Json(new { success = true, message = "Laboratorio eliminado correctamente." });

            return Json(new { success = false, message = apiResult?.Message ?? "Error al eliminar el laboratorio" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Excepción: {ex.Message}", details = ex.InnerException?.Message });
        }
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
