using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Guardians.Commands.CreateGuardian;


namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuardiansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GuardiansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGuardianCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllGuardiansQuery());
            return Ok(result);
        }
    }
}