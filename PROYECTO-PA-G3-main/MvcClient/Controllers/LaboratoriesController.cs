using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;

[Authorize]
public class LaboratoriesController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public IActionResult Index() => View();
    private HttpRequestMessage CreateRequest(HttpMethod method, string path, object? payload = null)
    {
        var endpoint = $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}/{path}";
        var request = new HttpRequestMessage(method, endpoint);
        var token = User.FindFirst("AccessToken")?.Value;
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (payload != null)
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        return request;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        using var request = CreateRequest(HttpMethod.Get, "GetAllLaboratories");

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
        var client = httpClientFactory.CreateClient("ApiClient");
        using var request = CreateRequest(HttpMethod.Get, $"GetLaboratoryById/{id}");

        using var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LaboratoryUpsertDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            using var request = CreateRequest(HttpMethod.Post, "CreateLaboratory", dto);

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
            var client = httpClientFactory.CreateClient("ApiClient");
            using var request = CreateRequest(HttpMethod.Put, $"UpdateLaboratory/{id}", dto);

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
            var client = httpClientFactory.CreateClient("ApiClient");
            using var request = CreateRequest(HttpMethod.Delete, $"DeleteLaboratory/{id}");

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
}