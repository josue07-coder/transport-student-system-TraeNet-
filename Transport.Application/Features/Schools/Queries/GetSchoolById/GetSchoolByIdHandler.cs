using MediatR;
using Transport.Application.Features.Schools.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Schools.Queries.GetSchoolById
{
    public class GetSchoolByIdHandler : IRequestHandler<GetSchoolByIdQuery, SchoolResponseDto>
    {
        private readonly ISchoolRepository _repository;

        public GetSchoolByIdHandler(ISchoolRepository repository)
        {
            _repository = repository;
        }

        public async Task<SchoolResponseDto> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
        {
            var school = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("School not found");

            return new SchoolResponseDto
            {
                Id = school.Id,
                Name = school.Name,
                DirectorName = school.DirectorName,
                Email = school.ContactEmail.Value,
                Phone = school.ContactPhone.Value,
                Street = school.Address.Street,
                City = school.Address.City,
                SectorId = school.SectorId
            };
        }
    }
}
