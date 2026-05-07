using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;

namespace Transport.Application.Features.Grades.Commands.CreateGrade
{
    public class CreateGradeHandler : IRequestHandler<CreateGradeCommand, Guid>
    {
        private readonly IGradeRepository _repo;

        public CreateGradeHandler(IGradeRepository repo)
        {
            _repo = repo;
        }

        public async Task<Guid> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
        {
            var grade = new Grade(request.Name, request.SchoolId);

            await _repo.AddAsync(grade);
            await _repo.SaveChangesAsync();

            return grade.Id;
        }
    }
}