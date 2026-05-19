using MediatR;

namespace Transport.Application.Features.Grades.Commands.UpdateGrade
{
    public class UpdateGradeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
