using MediatR;
using Transport.Application.Features.Schools.DTOs;

namespace Transport.Application.Features.Schools.Queries.GetSchoolById
{
    public record GetSchoolByIdQuery(Guid Id) : IRequest<SchoolResponseDto>;
}
