using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Guardians.DTOs;
using Transport.Application.Interfaces;

public class GetAllGuardiansHandler : IRequestHandler<GetAllGuardiansQuery, PaginatedResponse<GuardianResponseDto>>
{
    private readonly IGuardianRepository _repo;

    public GetAllGuardiansHandler(IGuardianRepository repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedResponse<GuardianResponseDto>> Handle(GetAllGuardiansQuery request, CancellationToken cancellationToken)
    {
        var guardians = await _repo.GetPagedAsync(request.PageNumber, request.PageSize);
        var items = guardians.Items.Select(g => new GuardianResponseDto
        {
            Id = g.Id,
            FullName = $"{g.FirstName} {g.LastName}",
            Phone = g.Phone,
            Address = $"{g.Address.Street}, {g.Address.City}"
        });

        return new PaginatedResponse<GuardianResponseDto>(
            items,
            guardians.TotalCount,
            guardians.PageNumber,
            guardians.PageSize);
    }
}
