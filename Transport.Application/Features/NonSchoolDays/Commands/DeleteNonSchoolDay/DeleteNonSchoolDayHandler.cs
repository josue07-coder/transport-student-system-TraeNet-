using MediatR;
using Transport.Application.Features.NonSchoolDays.Commands.DeactivateNonSchoolDay;

namespace Transport.Application.Features.NonSchoolDays.Commands.DeleteNonSchoolDay
{
    public class DeleteNonSchoolDayHandler : IRequestHandler<DeleteNonSchoolDayCommand, Unit>
    {
        private readonly IMediator _mediator;

        public DeleteNonSchoolDayHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeleteNonSchoolDayCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeactivateNonSchoolDayCommand { Id = request.Id }, cancellationToken);
            return Unit.Value;
        }
    }
}
