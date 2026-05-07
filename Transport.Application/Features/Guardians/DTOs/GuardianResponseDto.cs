namespace Transport.Application.Features.Guardians.DTOs
{
    public class GuardianResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}