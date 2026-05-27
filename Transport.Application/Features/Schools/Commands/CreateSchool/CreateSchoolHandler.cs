using MediatR;
using Transport.Application.Features.Schools.Commands.CreateSchool;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

public class CreateSchoolHandler : IRequestHandler<CreateSchoolCommand, Guid>
{
    private readonly ISchoolRepository _repo;
    private readonly ISectorRepository _sectorRepository;

    public CreateSchoolHandler(ISchoolRepository repo, ISectorRepository sectorRepository)
    {
        _repo = repo;
        _sectorRepository = sectorRepository;
    }

    public async Task<Guid> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        if (!await _sectorRepository.ExistsAsync(request.SectorId))
            throw new DomainException("Sector no encontrado");

        if (await _repo.ExistsByNameInSectorAsync(request.Name, request.SectorId))
            throw new DomainException("Ya existe una escuela con ese nombre en el sector");

        var school = new School(
            request.Name,
            request.DirectorName,
            Email.Create(request.Email),
            PhoneNumber.Create(request.Phone),
            Address.Create(request.Street, request.City),
            request.SectorId
        );

        await _repo.AddAsync(school);
        await _repo.SaveChangesAsync();

        return school.Id;
    }
}
