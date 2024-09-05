using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Controllers;

[ApiController]
[AllowAnonymous]
public class WebhookController : Controller
{
    private readonly IMemoryCache _memoryCache;

    public WebhookController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    
    [HttpGet("get-memory", Name = "GetMemory")]
    public async Task<IActionResult> GetMemory()
    {
        var keysAvailable = _memoryCache.TryGetValue("keys", out var keys);

        if (!keysAvailable)
            return Ok(new List<string>());

        var keysList = (List<string>)keys ?? new List<string>();

        var responseObject = new List<object>();
        
        foreach (var keyEntry in keysList)
        {
            var entryAvailable = _memoryCache.TryGetValue(keyEntry, out var entry);
            
            if (!entryAvailable)
                continue;
            
            responseObject.Add(entry);
        }

        return Ok(responseObject);
    }
    
    [HttpPost("post-webhook", Name = "PostWebhook")]
    public async Task<IActionResult> PostWebHook([FromBody] object body)
    {
        var headers = HttpContext.Request.Headers;
        var guid = Guid.NewGuid();

        var keysAvailable = _memoryCache.TryGetValue("keys", out var keys);

        var keysEntry = keysAvailable ? (List<string>)keys ?? new List<string>() : new List<string>();
        keysEntry.Add(guid.ToString());

        _memoryCache.Set("keys", keysEntry);

        _memoryCache.GetOrCreate(guid.ToString(), entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromHours(24);

            return new
            {
                Body = body,
                Signature = headers["x-spike-signature"]
            };
        });

        return NoContent();
    }
    
    [HttpPut("get-webhook", Name = "GetWebhook")]
    public async Task<IActionResult> GetWebhook([FromBody] object body)
    {
        var headers = HttpContext.Request.Headers;
        
        var guid = Guid.NewGuid();

        var keysAvailable = _memoryCache.TryGetValue("keys", out var keys);

        var keysEntry = keysAvailable ? (List<string>)keys : new List<string>();
        keysEntry.Add(guid.ToString());

        _memoryCache.Set("keys", keysEntry);
        
        _memoryCache.GetOrCreate(Guid.NewGuid(), entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromHours(24);

            return new
            {
                Body = body,
                Signature = headers["x-spike-signature"]
            };
        });

        return NoContent();
    }
}