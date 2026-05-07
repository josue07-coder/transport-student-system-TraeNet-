using MediatR;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Commands.DeleteStudent
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, Unit>
    {
        private readonly IStudentRepository _repo;

        public DeleteStudentHandler(IStudentRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _repo.GetByIdAsync(request.Id);

            if (student == null)
                throw new Exception("Student not found");

            student.Deactivate(); // Soft delete

            await _repo.SaveChangesAsync();

            return Unit.Value;
        }
    }
}