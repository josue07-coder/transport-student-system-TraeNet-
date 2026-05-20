using MediatR;

namespace Transport.Application.Features.Users.Commands.DeactivateUser
{
    public class DeactivateUserCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeactivateUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
