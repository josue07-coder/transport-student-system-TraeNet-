using MediatR;
using Transport.Application.Features.Schools.Commands.CreateSchool;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.ValueObjects;

public class CreateSchoolHandler : IRequestHandler<CreateSchoolCommand, Guid>
{
    private readonly ISchoolRepository _repo;

    public CreateSchoolHandler(ISchoolRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
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