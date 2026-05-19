using MediatR;

namespace Transport.Application.Features.Sectors.Commands.UpdateSector
{
    public class UpdateSectorCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public Guid? SchoolDistrictId { get; set; }
    }
}
