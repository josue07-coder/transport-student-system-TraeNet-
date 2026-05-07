using MediatR;
using Transport.Application.Features.Grades.DTOs;
using Transport.Application.Interfaces;

public class GetAllGradesHandler : IRequestHandler<GetAllGradesQuery, List<GradeResponseDto>>
{
    private readonly IGradeRepository _repo;

    public GetAllGradesHandler(IGradeRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<GradeResponseDto>> Handle(GetAllGradesQuery request, CancellationToken cancellationToken)
    {
        var grades = await _repo.GetAllAsync();

        return grades.Select(g => new GradeResponseDto
        {
            Id = g.Id,
            Name = g.Name,
            SchoolId = g.SchoolId
        }).ToList();
    }
}