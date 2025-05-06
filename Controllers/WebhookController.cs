using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyProjectIT15.Models;
using MyProjectIT15.Services;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly PayMongoSettings _settings;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        ApplicationDBContext context,
        IOptions<PayMongoSettings> options,
        ILogger<WebhookController> logger)
    {
        _context = context;
        _settings = options.Value;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post()
    {
        JObject payload;

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        _logger.LogInformation("Received Webhook Payload: {Body}", body);

        try
        {
            payload = JObject.Parse(body);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to parse webhook: " + ex.Message);
            return BadRequest();
        }

        var eventType = payload["data"]?["attributes"]?["type"]?.ToString();
        _logger.LogInformation("Webhook event type: {EventType}", eventType);

        string reference = payload["data"]?["attributes"]?["data"]?["attributes"]?["reference_number"]?.ToString();

        if (!string.IsNullOrEmpty(reference))
        {
            var payment = _context.Payments.FirstOrDefault(p => p.ReferenceNumber == reference);
            if (payment != null)
            {
                payment.Status = "paid";
                payment.LastWebhookEvent = eventType;
                payment.LastUpdatedAt = DateTime.UtcNow;

                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Payment with reference {Reference} marked as paid.", reference);
            }
        }

        return Ok(); // Prevents retry from PayMongo
    }

}