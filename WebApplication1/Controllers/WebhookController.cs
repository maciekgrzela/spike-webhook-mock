using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[AllowAnonymous]
public class WebhookController : Controller
{
    [HttpPost("post-webhook", Name = "PostWebhook")]
    public async Task<IActionResult> PostWebHook([FromBody] object body)
    {
        var headers = HttpContext.Request.Headers;

        return Ok(new
        {
            Body = body,
            Signature = headers["x-spike-signature"]
        });
    }
    
    [HttpGet("get-webhook", Name = "GetWebhook")]
    public async Task<IActionResult> GetWebhook([FromBody] object body)
    {
        var headers = HttpContext.Request.Headers;

        return Ok(new
        {
            Body = body,
            Signature = headers["x-spike-signature"]
        });
    }
}