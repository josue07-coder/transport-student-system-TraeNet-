using MediatR;

namespace Transport.Application.Features.Schools.Commands.DeleteSchool
{
    public class DeleteSchoolCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
