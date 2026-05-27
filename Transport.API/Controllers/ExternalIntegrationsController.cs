using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Interfaces;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/integrations")]
    public class ExternalIntegrationsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IMapService _mapService;
        private readonly IAuditService _auditService;

        public ExternalIntegrationsController(
            IEmailService emailService,
            ISmsService smsService,
            IWhatsAppService whatsAppService,
            IPushNotificationService pushNotificationService,
            IMapService mapService,
            IAuditService auditService)
        {
            _emailService = emailService;
            _smsService = smsService;
            _whatsAppService = whatsAppService;
            _pushNotificationService = pushNotificationService;
            _mapService = mapService;
            _auditService = auditService;
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(request.To, request.Subject, request.Body, cancellationToken);
            await _auditService.LogAsync("IntegrationTestEmail", "ExternalIntegration", null, null, request.To);
            return Ok(new { Success = true });
        }

        [HttpPost("test-sms")]
        public async Task<IActionResult> TestSms([FromBody] TestPhoneMessageRequest request, CancellationToken cancellationToken)
        {
            await _smsService.SendSmsAsync(request.PhoneNumber, request.Message, cancellationToken);
            await _auditService.LogAsync("IntegrationTestSms", "ExternalIntegration", null, null, request.PhoneNumber);
            return Ok(new { Success = true });
        }

        [HttpPost("test-whatsapp")]
        public async Task<IActionResult> TestWhatsApp([FromBody] TestPhoneMessageRequest request, CancellationToken cancellationToken)
        {
            await _whatsAppService.SendWhatsAppMessageAsync(request.PhoneNumber, request.Message, cancellationToken);
            await _auditService.LogAsync("IntegrationTestWhatsApp", "ExternalIntegration", null, null, request.PhoneNumber);
            return Ok(new { Success = true });
        }

        [HttpPost("test-push")]
        public async Task<IActionResult> TestPush([FromBody] TestPushRequest request, CancellationToken cancellationToken)
        {
            await _pushNotificationService.SendPushAsync(request.UserId, request.Title, request.Message, cancellationToken);
            await _auditService.LogAsync("IntegrationTestPush", "ExternalIntegration", request.UserId.ToString(), null, request.Title);
            return Ok(new { Success = true });
        }

        [HttpGet("test-distance")]
        public async Task<IActionResult> TestDistance(
            [FromQuery] decimal originLat,
            [FromQuery] decimal originLng,
            [FromQuery] decimal destinationLat,
            [FromQuery] decimal destinationLng,
            CancellationToken cancellationToken)
        {
            var distanceKm = await _mapService.CalculateDistanceAsync(originLat, originLng, destinationLat, destinationLng, cancellationToken);
            var estimatedTravelTime = await _mapService.EstimateTravelTimeAsync(originLat, originLng, destinationLat, destinationLng, cancellationToken);

            return Ok(new
            {
                DistanceKm = Math.Round(distanceKm, 2),
                EstimatedTravelTimeMinutes = estimatedTravelTime?.TotalMinutes
            });
        }

        public class TestEmailRequest
        {
            public string To { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }

        public class TestPhoneMessageRequest
        {
            public string PhoneNumber { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        public class TestPushRequest
        {
            public Guid UserId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }
    }
}
