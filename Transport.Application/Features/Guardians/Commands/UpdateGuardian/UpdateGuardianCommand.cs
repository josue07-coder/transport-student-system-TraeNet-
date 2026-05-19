using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Guardians.Commands.UpdateGuardian
{
    public class UpdateGuardianCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public Guid? SectorId { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
