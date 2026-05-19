using MediatR;
using Transport.Application.Features.Guardians.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Guardians.Queries.GetGuardianByDocument
{
    public class GetGuardianByDocumentHandler : IRequestHandler<GetGuardianByDocumentQuery, GuardianDetailDto>
    {
        private readonly IGuardianRepository _repository;

        public GetGuardianByDocumentHandler(IGuardianRepository repository)
        {
            _repository = repository;
        }

        public async Task<GuardianDetailDto> Handle(GetGuardianByDocumentQuery request, CancellationToken cancellationToken)
        {
            var guardian = await _repository.GetByDocumentAsync(request.DocumentNumber)
                ?? throw new DomainException("Guardian not found");

            return new GuardianDetailDto
            {
                Id = guardian.Id,
                DocumentType = guardian.DocumentType,
                DocumentNumber = guardian.DocumentNumber,
                FirstName = guardian.FirstName,
                LastName = guardian.LastName,
                FullName = $"{guardian.FirstName} {guardian.LastName}",
                Phone = guardian.Phone,
                Street = guardian.Address.Street,
                City = guardian.Address.City,
                Gender = guardian.Gender,
                SectorId = guardian.SectorId,
                SectorName = guardian.Sector?.Name,
                PhotoUrl = guardian.PhotoUrl,
                IsActive = guardian.IsActive
            };
        }
    }
}
