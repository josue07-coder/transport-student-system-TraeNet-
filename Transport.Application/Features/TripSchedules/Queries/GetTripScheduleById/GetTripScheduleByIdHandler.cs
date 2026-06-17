using MediatR;
using Transport.Application.Features.TripSchedules.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripSchedules.Queries.GetTripScheduleById
{
    public class GetTripScheduleByIdHandler : IRequestHandler<GetTripScheduleByIdQuery, TripScheduleDetailDto>
    {
        private readonly ITripScheduleRepository _repository;

        public GetTripScheduleByIdHandler(ITripScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<TripScheduleDetailDto> Handle(GetTripScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdWithDetailsAsync(request.Id)
                ?? throw new DomainException("Programación de viaje no encontrada");

            return TripScheduleMappings.ToDetailDto(schedule);
        }
    }
}
