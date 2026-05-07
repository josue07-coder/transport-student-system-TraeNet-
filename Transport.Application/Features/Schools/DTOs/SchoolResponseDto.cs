namespace Transport.Application.Features.Schools.DTOs
{
    public class SchoolResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DirectorName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Street { get; set; }
        public string City { get; set; }

        public Guid SectorId { get; set; }
    }
}