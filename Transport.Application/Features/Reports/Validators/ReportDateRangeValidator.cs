using FluentValidation;
using Transport.Application.Features.Reports.Queries.GetAuditSummaryReport;
using Transport.Application.Features.Reports.Queries.GetDriversPerformanceReport;
using Transport.Application.Features.Reports.Queries.GetIncidentsReport;
using Transport.Application.Features.Reports.Queries.GetTripsReport;
using Transport.Application.Features.Reports.Queries.GetVehiclesUsageReport;

namespace Transport.Application.Features.Reports.Validators
{
    public class GetTripsReportValidator : AbstractValidator<GetTripsReportQuery>
    {
        public GetTripsReportValidator() => this.AddDateRangeRules();
    }

    public class GetIncidentsReportValidator : AbstractValidator<GetIncidentsReportQuery>
    {
        public GetIncidentsReportValidator() => this.AddDateRangeRules();
    }

    public class GetDriversPerformanceReportValidator : AbstractValidator<GetDriversPerformanceReportQuery>
    {
        public GetDriversPerformanceReportValidator() => this.AddDateRangeRules();
    }

    public class GetVehiclesUsageReportValidator : AbstractValidator<GetVehiclesUsageReportQuery>
    {
        public GetVehiclesUsageReportValidator() => this.AddDateRangeRules();
    }

    public class GetAuditSummaryReportValidator : AbstractValidator<GetAuditSummaryReportQuery>
    {
        public GetAuditSummaryReportValidator() => this.AddDateRangeRules();
    }

    internal static class ReportDateRangeRules
    {
        public static void AddDateRangeRules<T>(this AbstractValidator<T> validator)
            where T : IDateRangeReportQuery
        {
            validator.RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("La fecha inicial es requerida");

            validator.RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("La fecha final es requerida")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("La fecha final no puede ser menor que la fecha inicial")
                .Must((request, endDate) => endDate <= request.StartDate.AddYears(1))
                .WithMessage("El rango máximo permitido es de 1 año");
        }
    }

    public interface IDateRangeReportQuery
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}
