using MediatR;

namespace Transport.Application.Features.Guardians.Commands.CreateGuardian
{
    public class CreateGuardianCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public Guid SectorId { get; set; }
    }
}