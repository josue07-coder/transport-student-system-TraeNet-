using MediatR;

namespace Transport.Application.Features.Guardians.Commands.DeleteGuardian
{
    public class DeleteGuardianCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
