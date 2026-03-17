using Microsoft.AspNetCore.Mvc;

namespace ProjectLucien.Api.Shared.Health;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow
        });
    }
}
