using MediatR;
using Transport.Application.Features.Grades.DTOs;

public class GetAllGradesQuery : IRequest<List<GradeResponseDto>>
{
}