using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.NonSchoolDays.Commands.DeactivateNonSchoolDay
{
    public class DeactivateNonSchoolDayHandler : IRequestHandler<DeactivateNonSchoolDayCommand, Unit>
    {
        private readonly INonSchoolDayRepository _repository;
        private readonly IAuditService _auditService;

        public DeactivateNonSchoolDayHandler(INonSchoolDayRepository repository, IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeactivateNonSchoolDayCommand request, CancellationToken cancellationToken)
        {
            var day = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Día no escolar no encontrado");

            day.Deactivate();
            await _repository.SaveChangesAsync();
            await _auditService.LogAsync("NonSchoolDayDeactivated", "NonSchoolDay", day.Id.ToString());
            return Unit.Value;
        }
    }
}
