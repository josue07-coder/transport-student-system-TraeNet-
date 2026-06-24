using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Queries.SearchAttendanceStudents
{
    public class SearchAttendanceStudentsHandler : IRequestHandler<SearchAttendanceStudentsQuery, List<AttendanceStudentSearchResultDto>>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly ITripOperationAuthorizationService _authorizationService;

        public SearchAttendanceStudentsHandler(
            ITripRepository tripRepository,
            IStudentRepository studentRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            ITripOperationAuthorizationService authorizationService)
        {
            _tripRepository = tripRepository;
            _studentRepository = studentRepository;
            _attendanceRepository = attendanceRepository;
            _authorizationService = authorizationService;
        }

        public async Task<List<AttendanceStudentSearchResultDto>> Handle(SearchAttendanceStudentsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return new List<AttendanceStudentSearchResultDto>();

            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await _authorizationService.EnsureCanManageTripAttendanceAsync(trip);

            var officialStudentIds = trip.RouteAssignment.Students
                .Select(assignment => assignment.StudentId)
                .ToHashSet();

            var students = await _studentRepository.SearchForAttendanceAsync(request.Query);
            var results = new List<AttendanceStudentSearchResultDto>();

            foreach (var student in students)
            {
                results.Add(new AttendanceStudentSearchResultDto
                {
                    StudentId = student.Id,
                    FullName = $"{student.FirstName} {student.LastName}",
                    StudentCode = student.StudentCode.Value,
                    GuardianId = student.GuardianId,
                    GuardianName = student.Guardian == null ? string.Empty : $"{student.Guardian.FirstName} {student.Guardian.LastName}",
                    GuardianDocumentNumber = student.Guardian?.DocumentNumber,
                    SchoolName = student.School?.Name,
                    IsAssignedToRoute = officialStudentIds.Contains(student.Id),
                    AlreadyRegisteredInTrip = await _attendanceRepository.ExistsAsync(request.TripId, student.Id)
                });
            }

            return results
                .OrderByDescending(result => result.IsAssignedToRoute)
                .ThenBy(result => result.FullName)
                .ToList();
        }
    }
}
