using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Json;

[Route("api/[controller]")]
[ApiController]
public class ChartController(IChartService chartService) : ControllerBase
{
    private readonly IChartService chartService = chartService;

    [HttpGet("pie")]
    public async Task<IActionResult> GetPieChart()
    {
        var result = await chartService.GeneratePieChartAsync();
        if (result.IsSuccess && result.Data != null)
        {
            return Ok(new { ImageBase64 = result.Data });
        }
        else
        {
            return BadRequest(result.Error);
        }
    }
}
