namespace Transport.Application.Features.Reports.DTOs
{
    public class StudentsByRouteReportDto
    {
        public Guid RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public List<StudentRouteReportItemDto> Students { get; set; } = new();
    }
}
