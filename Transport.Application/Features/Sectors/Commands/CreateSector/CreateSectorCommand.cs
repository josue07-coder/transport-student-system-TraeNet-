using MediatR;

namespace Transport.Application.Features.Sectors.Commands.CreateSector
{
    public class CreateSectorCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
       
    }
}