using Microsoft.AspNetCore.Mvc;
using Pmjay.Api.Data;

namespace Pmjay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly AgraDataService _service;

        public HomeController(AgraDataService service)
        {
            _service = service;
        }

        // GET: api/Agra/total
        [HttpGet("total")]
        public async Task<ActionResult<int>> GetTotal()
        {
            var total = await _service.GetTotalAsync();
            return Ok(total);
        }

        // GET: api/Agra/page?page=1&pageSize=20&block=XYZ
        [HttpGet("page")]
        public async Task<ActionResult<List<Dictionary<string, object?>>>> GetPage(
            int page = 1, int pageSize = 20,
            string? block = null, string? village = null, string? ru = null,
            string? sourceType = null, string? memberStatus = null,
            string? familyStatus = null, string? search = null)
        {
            var results = await _service.GetPageAsync(page, pageSize, block, village, ru, sourceType, memberStatus, familyStatus, search);
            return Ok(results);
        }

        // GET: api/Agra/selected?page=1&pageSize=20
        [HttpGet("selected")]
        public async Task<ActionResult<List<Agra1Dto>>> GetSelectedColumns(
            int page = 1, int pageSize = 20,
            string? block = null, string? village = null, string? ru = null,
            string? sourceType = null, string? memberStatus = null,
            string? familyStatus = null, string? search = null)
        {
            var results = await _service.GetSelectedColumnsAsync(page, pageSize, block, village, ru, sourceType, memberStatus, familyStatus, search);
            return Ok(results);
        }

        // GET: api/Agra/summary?block=XYZ
        [HttpGet("summary")]
        public async Task<ActionResult<SummaryDto>> GetFilteredSummary(
            string? block = null, string? village = null, string? ru = null,
            string? sourceType = null, string? memberStatus = null,
            string? familyStatus = null, string? search = null)
        {
            var summary = await _service.GetFilteredSummaryAsync(block, village, ru, sourceType, memberStatus, familyStatus, search);
            return Ok(summary);
        }
    }
}