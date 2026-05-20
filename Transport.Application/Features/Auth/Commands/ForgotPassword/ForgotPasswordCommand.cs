using MediatR;

namespace Transport.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<Unit>
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string PhoneOrEmail { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
