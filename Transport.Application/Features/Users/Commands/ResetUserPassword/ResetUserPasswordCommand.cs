using MediatR;

namespace Transport.Application.Features.Users.Commands.ResetUserPassword
{
    public class ResetUserPasswordCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
