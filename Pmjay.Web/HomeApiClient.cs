using Pmjay.Api.Data;
using Pmjay.Web.Data;
using System.Net.Http.Json;

namespace Pmjay.Web.ApiClients;
public class HomeApiClient
{
    private readonly HttpClient _http;

    public HomeApiClient(HttpClient http)
    {
        _http = http;
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
        var response = await _http.PostAsJsonAsync("api/verification/Upsert", verificationDetailRequest);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<VerificationDetailRequestDto>();
    }

}
