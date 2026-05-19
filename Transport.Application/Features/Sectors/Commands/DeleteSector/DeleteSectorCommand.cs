using MediatR;

namespace Transport.Application.Features.Sectors.Commands.DeleteSector
{
    public class DeleteSectorCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
