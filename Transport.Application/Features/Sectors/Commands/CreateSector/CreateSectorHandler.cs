using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;

namespace Transport.Application.Features.Sectors.Commands.CreateSector
{
    public class CreateSectorHandler : IRequestHandler<CreateSectorCommand, Guid>
    {
        private readonly ISectorRepository _repo;

        public CreateSectorHandler(ISectorRepository repo)
        {
            _repo = repo;
        }

        public async Task<Guid> Handle(CreateSectorCommand request, CancellationToken cancellationToken)
        {
            var sector = new Sector(
                request.Name,
                request.Province,
                request.City
                
             );

            await _repo.AddAsync(sector);
            await _repo.SaveChangesAsync();

            return sector.Id;
        }
    }
}