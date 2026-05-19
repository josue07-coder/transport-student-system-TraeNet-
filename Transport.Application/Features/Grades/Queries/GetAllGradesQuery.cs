using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Grades.DTOs;

public class GetAllGradesQuery : PaginationRequest, IRequest<PaginatedResponse<GradeResponseDto>>
{
}
