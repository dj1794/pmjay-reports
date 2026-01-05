using Microsoft.JSInterop;
using Pmjay.Api.Data;
using Pmjay.Web.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Pmjay.Web.ApiClients;
public class HomeApiClient
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public HomeApiClient(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
        // fire-and-forget is OK here, we only set header
        _ = SetTokenOnceAsync(js);
    }

    private async Task SetTokenOnceAsync(IJSRuntime js)
    {
        // If already added, do nothing
        if (_http.DefaultRequestHeaders.Authorization != null)
            return;

        var token = await js.InvokeAsync<string>(
            "localStorage.getItem", "token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
    public async Task<SummaryDto> GetSummaryAsync(
        string? block, string? village, string? ru,
        string? sourceType, string? memberStatus,
        string? familyStatus, string? search)
    {
        var url =
            $"api/home/summary" +
            $"?block={Uri.EscapeDataString(block ?? "")}" +
            $"&village={Uri.EscapeDataString(village ?? "")}" +
            $"&ru={Uri.EscapeDataString(ru ?? "")}" +
            $"&sourceType={Uri.EscapeDataString(sourceType ?? "")}" +
            $"&memberStatus={Uri.EscapeDataString(memberStatus ?? "")}" +
            $"&familyStatus={Uri.EscapeDataString(familyStatus ?? "")}" +
            $"&search={Uri.EscapeDataString(search ?? "")}";
        return await _http.GetFromJsonAsync<SummaryDto>(url)
               ?? new SummaryDto();
    }

    public async Task<List<Dictionary<string, object?>>> GetPageAsync(
        int page, int pageSize,
        string? block, string? village, string? ru,
        string? sourceType, string? memberStatus,
        string? familyStatus, string? search)
    {
        var url =
            $"api/home/page" +
            $"?page={page}&pageSize={pageSize}" +
            $"&block={Uri.EscapeDataString(block ?? "")}" +
            $"&village={Uri.EscapeDataString(village ?? "")}" +
            $"&ru={Uri.EscapeDataString(ru ?? "")}" +
            $"&sourceType={Uri.EscapeDataString(sourceType ?? "")}" +
            $"&memberStatus={Uri.EscapeDataString(memberStatus ?? "")}" +
            $"&familyStatus={Uri.EscapeDataString(familyStatus ?? "")}" +
            $"&search={Uri.EscapeDataString(search ?? "")}";
        return await _http.GetFromJsonAsync<List<Dictionary<string, object?>>>(url)
               ?? new();
    }
    public async Task<VerificationDetailRequestDto> SaveVerificationAsync(VerificationDetailRequestDto verificationDetailRequest)
    {
        var response = await _http.PostAsJsonAsync("api/verification", verificationDetailRequest);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<VerificationDetailRequestDto>();
    }

}
