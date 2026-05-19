using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Sectors.DTOs;
using Transport.Application.Interfaces;

public class GetAllSectorsHandler : IRequestHandler<GetAllSectorsQuery, PaginatedResponse<SectorResponseDto>>
{
    private readonly ISectorRepository _repo;

    public GetAllSectorsHandler(ISectorRepository repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedResponse<SectorResponseDto>> Handle(GetAllSectorsQuery request, CancellationToken cancellationToken)
    {
        var sectors = await _repo.GetPagedAsync(request.PageNumber, request.PageSize);
        var items = sectors.Items.Select(s => new SectorResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Province = s.Province,
            City = s.City
        });

        return new PaginatedResponse<SectorResponseDto>(
            items,
            sectors.TotalCount,
            sectors.PageNumber,
            sectors.PageSize);
    }
}
