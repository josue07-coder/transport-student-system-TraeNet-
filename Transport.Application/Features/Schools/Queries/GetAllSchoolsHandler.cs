using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Schools.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Schools.Queries.GetAllSchools
{
    public class GetAllSchoolsHandler : IRequestHandler<GetAllSchoolsQuery, PaginatedResponse<SchoolResponseDto>>
    {
        private readonly ISchoolRepository _repo;

        public GetAllSchoolsHandler(ISchoolRepository repo)
        {
            _repo = repo;
        }

        public async Task<PaginatedResponse<SchoolResponseDto>> Handle(GetAllSchoolsQuery request, CancellationToken cancellationToken)
        {
            var schools = await _repo.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = schools.Items.Select(s => new SchoolResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                DirectorName = s.DirectorName,
                Email = s.ContactEmail.Value,
                Phone = s.ContactPhone.Value,
                Street = s.Address.Street,
                City = s.Address.City,
                SectorId = s.SectorId
            });

            return new PaginatedResponse<SchoolResponseDto>(
                items,
                schools.TotalCount,
                schools.PageNumber,
                schools.PageSize);
        }
    }
}
