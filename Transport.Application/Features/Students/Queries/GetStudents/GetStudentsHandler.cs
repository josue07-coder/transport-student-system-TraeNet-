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

        public GetAllStudentsHandler(IStudentRepository repo)
        {
            _repo = repo;
        }

        public async Task<PaginatedResponse<StudentResponseDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _repo.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = students.Items.Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FullName = $"{s.FirstName} {s.LastName}",
                SchoolId = s.SchoolId,
                GradeId = s.GradeId,
                GuardianId = s.GuardianId
            });

            return new PaginatedResponse<StudentResponseDto>(
                items,
                students.TotalCount,
                students.PageNumber,
                students.PageSize);
        }
    }
}
