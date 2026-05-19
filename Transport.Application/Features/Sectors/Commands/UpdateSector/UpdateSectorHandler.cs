using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Sectors.Commands.UpdateSector
{
    public class UpdateSectorHandler : IRequestHandler<UpdateSectorCommand, Unit>
    {
        private readonly ISectorRepository _repository;

        public UpdateSectorHandler(ISectorRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateSectorCommand request, CancellationToken cancellationToken)
        {
            var sector = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Sector not found");

            sector.SetName(request.Name);
            sector.UpdateLocation(request.City, request.Province);
            sector.ChangeSchoolDistrict(request.SchoolDistrictId);

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
