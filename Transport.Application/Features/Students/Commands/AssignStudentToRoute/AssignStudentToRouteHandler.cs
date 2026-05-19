//using MediatR;
//using Transport.Application.Interfaces;
//using Transport.Domain.Exceptions;

//namespace Transport.Application.Features.Students.Commands.AssignStudentToRoute
//{
//    public class AssignStudentToRouteHandler : IRequestHandler<AssignStudentToRouteCommand, Unit>
//    {
//        private readonly IStudentRepository _studentRepository;
//        private readonly IRouteAssignmentRepository _routeRepository;

//        public AssignStudentToRouteHandler(
//            IStudentRepository studentRepository,
//            IRouteAssignmentRepository routeRepository)
//        {
//            _studentRepository = studentRepository;
//            _routeRepository = routeRepository;
//        }

//        public async Task<Unit> Handle(AssignStudentToRouteCommand request, CancellationToken cancellationToken)
//        {
//            var student = await _studentRepository.GetByIdAsync(request.StudentId)
//                ?? throw new DomainException("No se encotro el estudiante");

//            var route = await _routeRepository.GetByIdAsync(request.RouteAssignmentId)
//                ?? throw new DomainException("No se encontro la ruta");

//            route.AssignStudent(student.Id);

//            await _routeRepository.SaveChangesAsync();

//            return Unit.Value;
//        }
//    }
//}
