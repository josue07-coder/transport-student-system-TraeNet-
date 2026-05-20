using MediatR;

namespace Transport.Application.Features.Users.Commands.ActivateUser
{
    public class ActivateUserCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public ActivateUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
