using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Grades.DTOs;
using Transport.Application.Interfaces;

public class GetAllGradesHandler : IRequestHandler<GetAllGradesQuery, PaginatedResponse<GradeResponseDto>>
{
    private readonly IGradeRepository _repo;

    public GetAllGradesHandler(IGradeRepository repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedResponse<GradeResponseDto>> Handle(GetAllGradesQuery request, CancellationToken cancellationToken)
    {
        var grades = await _repo.GetPagedAsync(request.PageNumber, request.PageSize);
        var items = grades.Items.Select(g => new GradeResponseDto
        {
            Id = g.Id,
            Name = g.Name,
            SchoolId = g.SchoolId
        });

        return new PaginatedResponse<GradeResponseDto>(
            items,
            grades.TotalCount,
            grades.PageNumber,
            grades.PageSize);
    }
}
