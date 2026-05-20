using MediatR;

namespace Transport.Application.Features.Me.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Unit>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
