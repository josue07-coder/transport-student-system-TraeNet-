using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.Features.Reports.Queries.GetAuditSummaryReport;
using Transport.Application.Features.Reports.Queries.GetDashboardReport;
using Transport.Application.Features.Reports.Queries.GetDriversPerformanceReport;
using Transport.Application.Features.Reports.Queries.GetIncidentsReport;
using Transport.Application.Features.Reports.Queries.GetStudentsByRouteReport;
using Transport.Application.Features.Reports.Queries.GetTripsReport;
using Transport.Application.Features.Reports.Queries.GetVehiclesUsageReport;

namespace Transport.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor")]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _mediator.Send(new GetDashboardReportQuery());
            return Ok(result);
        }

        [HttpGet("trips")]
        public async Task<IActionResult> GetTrips([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetTripsReportQuery(startDate, endDate));
            return Ok(result);
        }

        [HttpGet("students-by-route/{routeId}")]
        public async Task<IActionResult> GetStudentsByRoute(Guid routeId)
        {
            var result = await _mediator.Send(new GetStudentsByRouteReportQuery(routeId));
            return Ok(result);
        }

        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetIncidentsReportQuery(startDate, endDate));
            return Ok(result);
        }

        [HttpGet("drivers-performance")]
        public async Task<IActionResult> GetDriversPerformance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetDriversPerformanceReportQuery(startDate, endDate));
            return Ok(result);
        }

        [HttpGet("vehicles-usage")]
        public async Task<IActionResult> GetVehiclesUsage([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetVehiclesUsageReportQuery(startDate, endDate));
            return Ok(result);
        }

        [HttpGet("audit-summary")]
        public async Task<IActionResult> GetAuditSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetAuditSummaryReportQuery(startDate, endDate));
            return Ok(result);
        }
    }
}
