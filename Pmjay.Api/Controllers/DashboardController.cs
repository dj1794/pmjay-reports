using Pmjay.Api.Data;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service)
    {
        _service = service;
    }

    [HttpGet("blocks")]
    public async Task<IActionResult> GetBlockDashboard()
    {
        var data = await _service.GetDashboardAsync();
        return Ok(data);
    }
}