using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudents
{
    public class GetAllStudentsQuery : PaginationRequest, IRequest<PaginatedResponse<StudentResponseDto>>
    {
    }
}
