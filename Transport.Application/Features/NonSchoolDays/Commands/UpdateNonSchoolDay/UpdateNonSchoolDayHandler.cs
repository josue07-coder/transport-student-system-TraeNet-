using MediatR;
using System.Text.Json;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.NonSchoolDays.Commands.UpdateNonSchoolDay
{
    public class UpdateNonSchoolDayHandler : IRequestHandler<UpdateNonSchoolDayCommand, Unit>
    {
        private readonly INonSchoolDayRepository _repository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IAuditService _auditService;

        public UpdateNonSchoolDayHandler(
            INonSchoolDayRepository repository,
            ISchoolRepository schoolRepository,
            IAuditService auditService)
        {
            _repository = repository;
            _schoolRepository = schoolRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(UpdateNonSchoolDayCommand request, CancellationToken cancellationToken)
        {
            var day = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Día no escolar no encontrado");

            if (request.SchoolId.HasValue && !await _schoolRepository.ExistsAsync(request.SchoolId.Value))
                throw new DomainException("Escuela no encontrada");

            if (day.IsActive && await _repository.ExistsActiveAsync(request.Date, request.SchoolId, day.Id))
                throw new DomainException("Ya existe un día no escolar activo para esa fecha y escuela");

            var oldValues = JsonSerializer.Serialize(new { day.Date, day.SchoolId, day.ReasonType, day.Reason });
            day.Update(request.Date, request.ReasonType, request.Reason, request.SchoolId);

            await _repository.SaveChangesAsync();
            await _auditService.LogAsync(
                "NonSchoolDayUpdated",
                "NonSchoolDay",
                day.Id.ToString(),
                oldValues,
                JsonSerializer.Serialize(new { day.Date, day.SchoolId, day.ReasonType, day.Reason }));

            return Unit.Value;
        }
    }
}
