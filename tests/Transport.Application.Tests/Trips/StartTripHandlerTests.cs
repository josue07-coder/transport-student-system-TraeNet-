using FluentAssertions;
using Moq;
using Transport.Application.Features.Trips.Commands.StartTrip;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Trips;

public class StartTripHandlerTests
{
    [Fact]
    public async Task StartTrip_Fails_WhenTripAlreadyActive()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var student = DomainTestFactory.ActiveStudent(assignment.Route.SchoolId);
        assignment.AssignStudent(student.Id);
        ReflectionHelper.SetProperty(assignment.Students.Single(), nameof(StudentRouteAssignment.Student), student);
        var handler = CreateHandler(assignment, hasActiveTrip: true);

        var act = () => handler.Handle(new StartTripCommand { RouteAssignmentId = assignment.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task StartTrip_Fails_WhenRouteHasNoStops()
    {
        var route = DomainTestFactory.ActiveRoute(withStops: false);
        var assignment = new RouteAssignment(route.Id, Guid.NewGuid(), Guid.NewGuid(), 2);
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.Route), route);
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.Driver), DomainTestFactory.ActiveDriver());
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.Vehicle), DomainTestFactory.ActiveVehicle());
        assignment.AssignStudent(Guid.NewGuid());

        var handler = CreateHandler(assignment, hasActiveTrip: false);

        var act = () => handler.Handle(new StartTripCommand { RouteAssignmentId = assignment.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task StartTrip_SavesTripAsInProgress()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var student = DomainTestFactory.ActiveStudent(assignment.Route.SchoolId);
        assignment.AssignStudent(student.Id);
        ReflectionHelper.SetProperty(assignment.Students.Single(), nameof(StudentRouteAssignment.Student), student);
        var trips = new Mock<ITripRepository>();
        var attendances = new Mock<ITripStudentAttendanceRepository>();
        Trip? savedTrip = null;
        var snapshot = new List<TripStudentAttendance>();
        trips.Setup(x => x.HasActiveTripAsync(assignment.Id)).ReturnsAsync(false);
        trips.Setup(x => x.AddAsync(It.IsAny<Trip>()))
            .Callback<Trip>(trip => savedTrip = trip)
            .Returns(Task.CompletedTask);
        trips.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        attendances.Setup(x => x.ExistsForTripAsync(It.IsAny<Guid>())).ReturnsAsync(false);
        attendances.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<TripStudentAttendance>>()))
            .Callback<IEnumerable<TripStudentAttendance>>(items => snapshot.AddRange(items))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler(assignment, trips, attendances);

        await handler.Handle(new StartTripCommand { RouteAssignmentId = assignment.Id }, CancellationToken.None);

        savedTrip.Should().NotBeNull();
        savedTrip!.Status.Should().Be(TripStatus.InProgress);
        snapshot.Should().ContainSingle();
        snapshot.Single().StudentId.Should().Be(student.Id);
        snapshot.Single().Status.Should().Be(TripAttendanceStatus.Expected);
    }

    [Fact]
    public async Task StartTrip_Fails_WhenSnapshotAlreadyExists()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var student = DomainTestFactory.ActiveStudent(assignment.Route.SchoolId);
        assignment.AssignStudent(student.Id);
        ReflectionHelper.SetProperty(assignment.Students.Single(), nameof(StudentRouteAssignment.Student), student);

        var trips = new Mock<ITripRepository>();
        var attendances = new Mock<ITripStudentAttendanceRepository>();
        trips.Setup(x => x.HasActiveTripAsync(assignment.Id)).ReturnsAsync(false);
        attendances.Setup(x => x.ExistsForTripAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        var handler = CreateHandler(assignment, trips, attendances);

        var act = () => handler.Handle(new StartTripCommand { RouteAssignmentId = assignment.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    private static StartTripHandler CreateHandler(RouteAssignment assignment, bool hasActiveTrip)
    {
        var trips = new Mock<ITripRepository>();
        trips.Setup(x => x.HasActiveTripAsync(assignment.Id)).ReturnsAsync(hasActiveTrip);

        return CreateHandler(assignment, trips);
    }

    private static StartTripHandler CreateHandler(
        RouteAssignment assignment,
        Mock<ITripRepository> trips,
        Mock<ITripStudentAttendanceRepository>? attendances = null)
    {
        var assignments = new Mock<IRouteAssignmentRepository>();
        if (attendances is null)
        {
            attendances = new Mock<ITripStudentAttendanceRepository>();
            attendances.Setup(x => x.ExistsForTripAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            attendances.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<TripStudentAttendance>>())).Returns(Task.CompletedTask);
        }
        var audit = new Mock<IAuditService>();
        var notifications = new Mock<INotificationService>();
        var users = new Mock<IUserRepository>();

        assignments.Setup(x => x.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        audit.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        notifications.Setup(x => x.NotifyUsersAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<NotificationType>(),
                It.IsAny<NotificationPriority>(),
                It.IsAny<string?>(),
                It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        return new StartTripHandler(assignments.Object, trips.Object, attendances.Object, audit.Object, notifications.Object, users.Object);
    }
}
