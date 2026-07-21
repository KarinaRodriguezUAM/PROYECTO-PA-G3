using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;

[Authorize]
public class DashboardController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ApiClient");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private bool IsAdministrator()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
        return string.Equals(roleClaim, "Administrator", StringComparison.OrdinalIgnoreCase) || User.IsInRole("Administrator");
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!IsAdministrator())
        {
            TempData["ErrorMessage"] = "Acceso denegado. El Dashboard de Métricas Operativas es exclusivo para usuarios con rol Administrator.";
            return RedirectToAction("Index", "Home");
        }

        var viewModel = new DashboardViewModel();

        try
        {
            // General Summary
            var summaryResponse = await _client.GetAsync("api/Dashboard/GeneralSummary", ct);
            if (summaryResponse.IsSuccessStatusCode)
            {
                var json = await summaryResponse.Content.ReadAsStringAsync(ct);
                var res = JsonSerializer.Deserialize<ApiResponse<GeneralSummaryModel>>(json, JsonOptions);
                if (res?.Result != null) viewModel.Summary = res.Result;
            }

            // Reports By Lab
            var labResponse = await _client.GetAsync("api/Dashboard/ReportsByLab", ct);
            if (labResponse.IsSuccessStatusCode)
            {
                var json = await labResponse.Content.ReadAsStringAsync(ct);
                var res = JsonSerializer.Deserialize<ApiResponse<List<ReportsByLabModel>>>(json, JsonOptions);
                if (res?.Result != null) viewModel.ReportsByLab = res.Result;
            }

            // Reports By Technician
            var techResponse = await _client.GetAsync("api/Dashboard/ReportsByTechnician", ct);
            if (techResponse.IsSuccessStatusCode)
            {
                var json = await techResponse.Content.ReadAsStringAsync(ct);
                var res = JsonSerializer.Deserialize<ApiResponse<List<ReportsByTechnicianModel>>>(json, JsonOptions);
                if (res?.Result != null) viewModel.ReportsByTechnician = res.Result;
            }

            // Reports By Status
            var statusResponse = await _client.GetAsync("api/Dashboard/ReportsByStatus", ct);
            if (statusResponse.IsSuccessStatusCode)
            {
                var json = await statusResponse.Content.ReadAsStringAsync(ct);
                var res = JsonSerializer.Deserialize<ApiResponse<List<ReportsByStatusModel>>>(json, JsonOptions);
                if (res?.Result != null) viewModel.ReportsByStatus = res.Result;
            }

            // Average Resolution Time
            var timeResponse = await _client.GetAsync("api/Dashboard/AverageResolutionTime", ct);
            if (timeResponse.IsSuccessStatusCode)
            {
                var json = await timeResponse.Content.ReadAsStringAsync(ct);
                var res = JsonSerializer.Deserialize<ApiResponse<AverageResolutionTimeModel>>(json, JsonOptions);
                if (res?.Result != null) viewModel.ResolutionTime = res.Result;
            }
        }
        catch
        {
            // Manejo de errores silencioso o fallback a datos por defecto
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetGeneralSummary(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/Dashboard/GeneralSummary", ct);
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(ct);
        return Content(content, "application/json");
    }

    [HttpGet]
    public async Task<IActionResult> GetReportsByLab(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/Dashboard/ReportsByLab", ct);
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(ct);
        return Content(content, "application/json");
    }

    [HttpGet]
    public async Task<IActionResult> GetReportsByTechnician(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/Dashboard/ReportsByTechnician", ct);
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(ct);
        return Content(content, "application/json");
    }

    [HttpGet]
    public async Task<IActionResult> GetAverageResolutionTime(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/Dashboard/AverageResolutionTime", ct);
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(ct);
        return Content(content, "application/json");
    }
}
