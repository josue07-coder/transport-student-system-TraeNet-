using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Grades.Commands.UpdateGrade
{
    public class UpdateGradeHandler : IRequestHandler<UpdateGradeCommand, Unit>
    {
        private readonly IGradeRepository _repository;

        public UpdateGradeHandler(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateGradeCommand request, CancellationToken cancellationToken)
        {
            var grade = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Grade not found");

            grade.UpdateName(request.Name);
            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
