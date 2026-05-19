using MediatR;
using Transport.Application.Features.Schools.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Schools.Queries.GetSchoolsBySector
{
    public class GetSchoolsBySectorHandler : IRequestHandler<GetSchoolsBySectorQuery, List<SchoolResponseDto>>
    {
        private readonly ISchoolRepository _repository;

        public GetSchoolsBySectorHandler(ISchoolRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SchoolResponseDto>> Handle(GetSchoolsBySectorQuery request, CancellationToken cancellationToken)
        {
            var schools = await _repository.GetBySectorAsync(request.SectorId);

            return schools.Select(s => new SchoolResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                DirectorName = s.DirectorName,
                Email = s.ContactEmail.Value,
                Phone = s.ContactPhone.Value,
                Street = s.Address.Street,
                City = s.Address.City,
                SectorId = s.SectorId
            }).ToList();
        }
    }
}
