using MediatR;

namespace Transport.Application.Features.Grades.Commands.DeleteGrade
{
    public class DeleteGradeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
