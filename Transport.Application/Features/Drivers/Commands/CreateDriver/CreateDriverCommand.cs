using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Drivers.Commands.CreateDriver
{
    public class CreateDriverCommand : IRequest<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
