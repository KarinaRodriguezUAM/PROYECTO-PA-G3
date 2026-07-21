using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Uam.LabHelpDesk.MvcClient.Models;

namespace Uam.LabHelpDesk.MvcClient.Controllers;

[Authorize]
public class FaultReportsController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ApiClient");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IActionResult Index() => View();

    public IActionResult Create() => View();

    public IActionResult Detail(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    public IActionResult Edit(int id)
    {
        ViewBag.Id = id;
        return View();
    }



    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var response = await _client.GetAsync("api/FaultReports/GetAllFaultReports", ct);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(ct);

        var apiResult =
            JsonSerializer.Deserialize<ApiResponse<List<FaultReportDto>>>(content, JsonOptions);

        return Json(apiResult?.Result ?? new List<FaultReportDto>());
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var response =
            await _client.GetAsync($"api/FaultReports/GetFaultReportById/{id}", ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FaultReportCreateDto dto, CancellationToken ct)
    {
        var response =
            await _client.PostAsJsonAsync("api/FaultReports/CreateFaultReport", dto, ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] FaultReportUpdateDto dto,
        CancellationToken ct)
    {
        var response =
            await _client.PutAsJsonAsync(
                $"api/FaultReports/UpdateFaultReport/{id}",
                dto,
                ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost]
    public async Task<IActionResult> Close(int id, CancellationToken ct)
    {
        var response =
            await _client.PostAsync(
                $"api/FaultReports/CloseFaultReport/{id}",
                null,
                ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return StatusCode((int)response.StatusCode, content);
    }

    public IActionResult UpdateStatus(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Assign(int id, CancellationToken ct)
    {
        var response = await _client.PostAsync($"api/FaultReports/AssignFaultReport/{id}", null, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatusPost(int id, [FromBody] UpdateFaultReportStatusDto dto, CancellationToken ct)
    {
        var response = await _client.PostAsJsonAsync($"api/FaultReports/UpdateFaultReportStatus/{id}", dto, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs(int id, CancellationToken ct)
    {
        var response = await _client.GetAsync($"api/FaultReports/GetLogsByFaultReport/{id}", ct);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(ct);

        var apiResult =
            JsonSerializer.Deserialize<ApiResponse<List<FaultReportStatusLogDto>>>(content, JsonOptions);

        return Json(apiResult?.Result ?? new List<FaultReportStatusLogDto>());
    }
}
