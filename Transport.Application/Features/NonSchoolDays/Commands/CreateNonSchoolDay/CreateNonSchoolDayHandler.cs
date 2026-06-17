using MediatR;
using System.Text.Json;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.NonSchoolDays.Commands.CreateNonSchoolDay
{
    public class CreateNonSchoolDayHandler : IRequestHandler<CreateNonSchoolDayCommand, Guid>
    {
        private readonly INonSchoolDayRepository _repository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IAuditService _auditService;

        public CreateNonSchoolDayHandler(
            INonSchoolDayRepository repository,
            ISchoolRepository schoolRepository,
            IAuditService auditService)
        {
            _repository = repository;
            _schoolRepository = schoolRepository;
            _auditService = auditService;
        }

        public async Task<Guid> Handle(CreateNonSchoolDayCommand request, CancellationToken cancellationToken)
        {
            if (request.SchoolId.HasValue && !await _schoolRepository.ExistsAsync(request.SchoolId.Value))
                throw new DomainException("Escuela no encontrada");

            if (await _repository.ExistsActiveAsync(request.Date, request.SchoolId))
                throw new DomainException("Ya existe un día no escolar activo para esa fecha y escuela");

            var day = new NonSchoolDay(request.Date, request.ReasonType, request.Reason, request.SchoolId);

            await _repository.AddAsync(day);
            await _repository.SaveChangesAsync();

            await _auditService.LogAsync(
                "NonSchoolDayCreated",
                "NonSchoolDay",
                day.Id.ToString(),
                null,
                JsonSerializer.Serialize(new { day.Date, day.SchoolId, day.ReasonType, day.Reason }));

            return day.Id;
        }
    }
}
