using FluentAssertions;
using Moq;
using Transport.Application.Features.RouteAssignments.Commands.AssignStudentToRouteAssignment;
using Transport.Application.Features.RouteAssignments.Commands.CreateRouteAssignment;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.RouteAssignments;

public class RouteAssignmentHandlerTests
{
    [Fact]
    public async Task CreateRouteAssignment_Fails_WhenVehicleIsInactive()
    {
        var route = DomainTestFactory.ActiveRoute();
        var driver = DomainTestFactory.ActiveDriver();
        var vehicle = DomainTestFactory.ActiveVehicle();
        vehicle.Deactivate();

        var assignments = new Mock<IRouteAssignmentRepository>();
        var routes = new Mock<IRouteRepository>();
        var drivers = new Mock<IDriverRepository>();
        var vehicles = new Mock<IVehicleRepository>();
        var assistants = new Mock<ITransportAssistantRepository>();
        var audit = new Mock<IAuditService>();

        routes.Setup(x => x.GetByIdWithStopsAsync(route.Id)).ReturnsAsync(route);
        drivers.Setup(x => x.GetByIdIncludingInactiveAsync(driver.Id)).ReturnsAsync(driver);
        vehicles.Setup(x => x.GetByIdAsync(vehicle.Id)).ReturnsAsync(vehicle);

        var handler = new CreateRouteAssignmentHandler(assignments.Object, routes.Object, drivers.Object, vehicles.Object, assistants.Object, audit.Object);

        var act = () => handler.Handle(new CreateRouteAssignmentCommand
        {
            RouteId = route.Id,
            DriverId = driver.Id,
            VehicleId = vehicle.Id,
            VehicleCapacity = 10
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AssignStudent_Fails_WhenCapacityIsExceeded()
    {
        var schoolId = Guid.NewGuid();
        var assignment = DomainTestFactory.AssignmentWithDetails(capacity: 1, schoolId);
        assignment.AssignStudent(Guid.NewGuid());
        var student = DomainTestFactory.ActiveStudent(schoolId);
        var handler = CreateAssignHandler(assignment, student);

        var act = () => handler.Handle(new AssignStudentToRouteAssignmentCommand
        {
            RouteAssignmentId = assignment.Id,
            StudentId = student.Id
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AssignStudent_Fails_WhenStudentBelongsToAnotherSchool()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails(capacity: 2, Guid.NewGuid());
        var student = DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var handler = CreateAssignHandler(assignment, student);

        var act = () => handler.Handle(new AssignStudentToRouteAssignmentCommand
        {
            RouteAssignmentId = assignment.Id,
            StudentId = student.Id
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    private static AssignStudentToRouteAssignmentHandler CreateAssignHandler(RouteAssignment assignment, Student student)
    {
        var assignments = new Mock<IRouteAssignmentRepository>();
        var students = new Mock<IStudentRepository>();
        var audit = new Mock<IAuditService>();
        var notifications = new Mock<INotificationService>();
        var users = new Mock<IUserRepository>();

        assignments.Setup(x => x.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        students.Setup(x => x.GetByIdIncludingInactiveAsync(student.Id)).ReturnsAsync(student);

        return new AssignStudentToRouteAssignmentHandler(
            assignments.Object,
            students.Object,
            audit.Object,
            notifications.Object,
            users.Object,
            new ImmediateUnitOfWork());
    }
}
