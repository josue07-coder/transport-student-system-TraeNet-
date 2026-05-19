using MediatR;

namespace Transport.Application.Features.Schools.Commands.UpdateSchool
{
    public class UpdateSchoolCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DirectorName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public Guid SectorId { get; set; }
        public string? Description { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
