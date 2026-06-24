using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Queries.GetTripAttendance
{
    public class GetTripAttendanceHandler : IRequestHandler<GetTripAttendanceQuery, TripAttendanceSummaryDto>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly ITripOperationAuthorizationService _authorizationService;

        public GetTripAttendanceHandler(
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            ITripOperationAuthorizationService authorizationService)
        {
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _authorizationService = authorizationService;
        }

        public async Task<TripAttendanceSummaryDto> Handle(GetTripAttendanceQuery request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _authorizationService.EnsureCanViewTripAsync(trip);

            var attendances = (await _attendanceRepository.GetByTripAsync(request.TripId))
                .Select(TripStudentAttendanceMappings.ToDto)
                .ToList();

            return new TripAttendanceSummaryDto
            {
                TripId = trip.Id,
                RouteAssignmentId = trip.RouteAssignmentId,
                RouteId = trip.RouteAssignment?.RouteId,
                RouteName = trip.RouteAssignment?.Route?.Name,
                OperationDate = trip.OperationDate,
                ScheduledDepartureTime = trip.ScheduledDepartureTime,
                StartTime = trip.StartTime,
                ExpectedCount = attendances.Count(x => x.IsExpectedPassenger),
                PresentCount = attendances.Count(x => x.Status == TripAttendanceStatus.Boarded || x.Status == TripAttendanceStatus.DroppedOff),
                AbsentCount = attendances.Count(x => x.Status == TripAttendanceStatus.Absent),
                ExceptionalCount = attendances.Count(x => !x.IsExpectedPassenger),
                ExpectedStudents = attendances.Where(x => x.IsExpectedPassenger).ToList(),
                PresentStudents = attendances.Where(x => x.Status == TripAttendanceStatus.Boarded || x.Status == TripAttendanceStatus.DroppedOff).ToList(),
                AbsentStudents = attendances.Where(x => x.Status == TripAttendanceStatus.Absent).ToList(),
                ExceptionalStudents = attendances.Where(x => !x.IsExpectedPassenger).ToList(),
                AllStudents = attendances
            };
        }
    }
}
