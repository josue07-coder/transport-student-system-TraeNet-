using Microsoft.AspNetCore.Mvc;
using Transport.Application.DTOs.User;
using Transport.Application.UseCases.Users;

namespace Transport.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController: ControllerBase
    {
        private readonly CreateUserUseCase _createUser;
        public UserController(CreateUserUseCase createUser)
        {
            _createUser = createUser;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDTO dto)
        {
            try
            {
                await _createUser.Execute(dto);
                return Ok("Usuario creado corectamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
}
