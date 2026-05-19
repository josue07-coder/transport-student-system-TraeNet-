using MediatR;
using Transport.Application.Features.Grades.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Grades.Queries.GetGradesBySchool
{
    public class GetGradesBySchoolHandler : IRequestHandler<GetGradesBySchoolQuery, List<GradeResponseDto>>
    {
        private readonly IGradeRepository _repository;

        public GetGradesBySchoolHandler(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GradeResponseDto>> Handle(GetGradesBySchoolQuery request, CancellationToken cancellationToken)
        {
            var grades = await _repository.GetBySchoolAsync(request.SchoolId);

            return grades.Select(g => new GradeResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                SchoolId = g.SchoolId
            }).ToList();
        }
    }
}
