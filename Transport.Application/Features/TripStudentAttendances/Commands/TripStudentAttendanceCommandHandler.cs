using MediatR;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentAbsent;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentBoarded;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentDroppedOff;
using Transport.Application.Features.TripStudentAttendances.Commands.UpdateTripStudentNotes;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Commands
{
    public class TripStudentAttendanceCommandHandler :
        IRequestHandler<MarkStudentBoardedCommand, Unit>,
        IRequestHandler<MarkStudentAbsentCommand, Unit>,
        IRequestHandler<MarkStudentDroppedOffCommand, Unit>,
        IRequestHandler<UpdateTripStudentNotesCommand, Unit>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;

        public TripStudentAttendanceCommandHandler(
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService)
        {
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(MarkStudentBoardedCommand request, CancellationToken cancellationToken)
        {
            var attendance = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            attendance.MarkBoarded(DateTime.UtcNow);
            await _attendanceRepository.SaveChangesAsync();
            await _auditService.LogAsync("TripStudentMarkedBoarded", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        public async Task<Unit> Handle(MarkStudentAbsentCommand request, CancellationToken cancellationToken)
        {
            var attendance = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            attendance.MarkAbsent(request.Notes);
            await _attendanceRepository.SaveChangesAsync();
            await _auditService.LogAsync("TripStudentMarkedAbsent", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        public async Task<Unit> Handle(MarkStudentDroppedOffCommand request, CancellationToken cancellationToken)
        {
            var attendance = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            attendance.MarkDroppedOff(DateTime.UtcNow);
            await _attendanceRepository.SaveChangesAsync();
            await _auditService.LogAsync("TripStudentMarkedDroppedOff", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateTripStudentNotesCommand request, CancellationToken cancellationToken)
        {
            var attendance = await GetAttendanceForUpdateAsync(request.TripId, request.StudentId);
            attendance.UpdateNotes(request.Notes);
            await _attendanceRepository.SaveChangesAsync();
            await _auditService.LogAsync("TripStudentNotesUpdated", "TripStudentAttendance", request.TripId.ToString(), null, $"{{\"StudentId\":\"{request.StudentId}\"}}");
            return Unit.Value;
        }

        private async Task<TripStudentAttendance> GetAttendanceForUpdateAsync(Guid tripId, Guid studentId)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(tripId)
                ?? throw new DomainException("Viaje no encontrado");

            await TripAttendanceAccess.EnsureCanUpdatePassengersAsync(trip, _userRepository, _currentUserService);

            return await _attendanceRepository.GetByTripAndStudentAsync(tripId, studentId)
                ?? throw new DomainException("Pasajero no encontrado en este viaje");
        }
    }
}
