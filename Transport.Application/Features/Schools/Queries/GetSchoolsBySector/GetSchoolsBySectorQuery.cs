using MediatR;
using Transport.Application.Features.Schools.DTOs;

namespace Transport.Application.Features.Schools.Queries.GetSchoolsBySector
{
    public record GetSchoolsBySectorQuery(Guid SectorId) : IRequest<List<SchoolResponseDto>>;
}
