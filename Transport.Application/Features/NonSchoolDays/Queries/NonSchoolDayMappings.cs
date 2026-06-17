using Transport.Application.Features.NonSchoolDays.DTOs;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Features.NonSchoolDays.Queries
{
    internal static class NonSchoolDayMappings
    {
        public static NonSchoolDayResponseDto ToResponseDto(NonSchoolDay day)
        {
            return new NonSchoolDayResponseDto
            {
                Id = day.Id,
                Date = day.Date,
                SchoolId = day.SchoolId,
                SchoolName = day.School?.Name,
                ReasonType = day.ReasonType,
                ReasonTypeLabel = GetReasonTypeLabel(day.ReasonType),
                Reason = day.Reason,
                IsActive = day.IsActive,
                CreatedAt = day.CreatedAt,
                UpdatedAt = day.UpdatedAt
            };
        }

        private static string GetReasonTypeLabel(NonSchoolDayReason reason)
        {
            return reason switch
            {
                NonSchoolDayReason.Holiday => "Feriado",
                NonSchoolDayReason.SchoolSuspension => "Suspensión de clases",
                NonSchoolDayReason.Weather => "Clima",
                NonSchoolDayReason.Administrative => "Administrativo",
                NonSchoolDayReason.Maintenance => "Mantenimiento",
                NonSchoolDayReason.Other => "Otro",
                _ => reason.ToString()
            };
        }
    }
}
