using MediatR;
using Transport.Application.Features.Guardians.DTOs;
using Transport.Application.Interfaces;

public class GetAllGuardiansHandler : IRequestHandler<GetAllGuardiansQuery, List<GuardianResponseDto>>
{
    private readonly IGuardianRepository _repo;

    public GetAllGuardiansHandler(IGuardianRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<GuardianResponseDto>> Handle(GetAllGuardiansQuery request, CancellationToken cancellationToken)
    {
        var guardians = await _repo.GetAllAsync();

        return guardians.Select(g => new GuardianResponseDto
        {
            Id = g.Id,
            FullName = $"{g.FirstName} {g.LastName}",
            Phone = g.Phone,
            Address = g.Address
        }).ToList();
    }
}