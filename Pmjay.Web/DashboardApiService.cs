using Pmjay.Web.Data;
using System.Net.Http.Json;

namespace Pmjay.Web
{
    public class DashboardApiService
    {
        private readonly HttpClient _http;

        public DashboardApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<BlockDashboardDto>> GetBlockDashboardAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<BlockDashboardDto>>(
                    "api/dashboard/blocks", ct
                );
                return result ?? new List<BlockDashboardDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error fetching dashboard: {ex.Message}");
                return new List<BlockDashboardDto>();
            }
        }
    }
}
