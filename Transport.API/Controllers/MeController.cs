using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Me.Commands.ChangePassword;
using Transport.Application.Features.Me.Commands.UpdateMeProfile;
using Transport.Application.Features.Me.Commands.UpdateProfilePhoto;
using Transport.Application.Features.Me.Queries.GetMe;
using Transport.Application.Features.Me.Queries.GetMyRouteAssignments;
using Transport.Application.Features.Me.Queries.GetMyStudents;
using Transport.Application.Features.Me.Queries.GetMyTrips;
using Transport.Application.Interfaces;
using Transport.API.Models;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/me")]
    public class MeController : ControllerBase
    {
        private const long MaxProfilePhotoSizeBytes = 5 * 1024 * 1024;
        private const long MaxProfilePhotoRequestSizeBytes = 6 * 1024 * 1024;
        private static readonly HashSet<string> AllowedProfilePhotoContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };
        private static readonly HashSet<string> AllowedProfilePhotoExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private readonly IMediator _mediator;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAuditService _auditService;
        private readonly ILogger<MeController> _logger;

        public MeController(
            IMediator mediator,
            IFileStorageService fileStorageService,
            IAuditService auditService,
            ILogger<MeController> logger)
        {
            _mediator = mediator;
            _fileStorageService = fileStorageService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            var result = await _mediator.Send(new GetMeQuery());
            return Ok(result);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetMyStudents()
        {
            var result = await _mediator.Send(new GetMyStudentsQuery());
            return Ok(result);
        }

        [HttpGet("route-assignments")]
        public async Task<IActionResult> GetMyRouteAssignments()
        {
            var result = await _mediator.Send(new GetMyRouteAssignmentsQuery());
            return Ok(result);
        }

        [HttpGet("trips")]
        public async Task<IActionResult> GetMyTrips()
        {
            var result = await _mediator.Send(new GetMyTripsQuery());
            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateMeProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("photo")]
        public async Task<IActionResult> UpdatePhoto([FromBody] UpdateProfilePhotoCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("photo/upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxProfilePhotoRequestSizeBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxProfilePhotoRequestSizeBytes)]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadProfilePhotoRequest request, CancellationToken cancellationToken)
        {
            var file = request.File;
            if (file is null || file.Length == 0)
            {
                return BadRequest("El archivo es requerido");
            }

            if (file.Length > MaxProfilePhotoSizeBytes)
            {
                return BadRequest("El archivo no puede exceder 5 MB");
            }

            if (!AllowedProfilePhotoContentTypes.Contains(file.ContentType))
            {
                return BadRequest("Tipo de archivo no permitido. Use JPG, PNG o WEBP");
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedProfilePhotoExtensions.Contains(extension))
            {
                return BadRequest("Extension de archivo no permitida. Use .jpg, .jpeg, .png o .webp");
            }

            var generatedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var storageFileName = Path.Combine("profile-photos", generatedFileName);

            await using var stream = file.OpenReadStream();
            var profileImageUrl = await _fileStorageService.UploadAsync(stream, storageFileName, file.ContentType, cancellationToken);

            await _mediator.Send(new UpdateProfilePhotoCommand
            {
                ProfileImageUrl = profileImageUrl
            }, cancellationToken);

            try
            {
                await _auditService.LogAsync(
                    "ProfilePhotoUploaded",
                    "User",
                    null,
                    null,
                    $"{{\"profileImageUrl\":\"{profileImageUrl}\"}}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Profile photo upload succeeded but audit logging failed");
            }

            return Ok(new
            {
                profileImageUrl
            });
        }
    }
}
