using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Students.DTOs;
using Transport.Application.Features.Students.Queries.GetStudents;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, PaginatedResponse<StudentResponseDto>>
    {
        private readonly IStudentRepository _repo;
        private readonly IVisibilityService _visibilityService;

        public GetAllStudentsHandler(IStudentRepository repo, IVisibilityService visibilityService)
        {
            _repo = repo;
            _visibilityService = visibilityService;
        }

        public async Task<PaginatedResponse<StudentResponseDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _repo.GetPagedAsync(request.PageNumber, request.PageSize);
            var visibleStudents = await _visibilityService.FilterStudentsAsync(students.Items);
            var items = visibleStudents.Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FullName = $"{s.FirstName} {s.LastName}",
                SchoolId = s.SchoolId,
                GradeId = s.GradeId,
                GuardianId = s.GuardianId
            });

            return new PaginatedResponse<StudentResponseDto>(
                items,
                visibleStudents.Count,
                students.PageNumber,
                students.PageSize);
        }
    }
}
