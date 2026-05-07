using MediatR;

namespace Transport.Application.Features.Grades.Commands.CreateGrade
{
    public class CreateGradeCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public Guid SchoolId { get; set; }
    }
}