using MediatR;
using Transport.Application.Features.Schools.DTOs;

namespace Transport.Application.Features.Schools.Queries.GetAllSchools
{
    public class GetAllSchoolsQuery : IRequest<List<SchoolResponseDto>>
    {
    }
}