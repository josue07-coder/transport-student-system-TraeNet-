using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Queries.GetTripPassengers
{
    public class GetTripPassengersHandler : IRequestHandler<GetTripPassengersQuery, List<TripStudentAttendanceDto>>
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetTripPassengersHandler(
            ITripRepository tripRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService)
        {
            _tripRepository = tripRepository;
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<TripStudentAttendanceDto>> Handle(GetTripPassengersQuery request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            await TripAttendanceAccess.EnsureCanViewPassengersAsync(trip, _userRepository, _currentUserService, _attendanceRepository);

            var attendances = await _attendanceRepository.GetByTripAsync(request.TripId);
            return attendances.Select(TripStudentAttendanceMappings.ToDto).ToList();
        }
    }
}
