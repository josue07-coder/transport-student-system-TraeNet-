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

    [Fact]
    public async Task StartTrip_Fails_WhenScheduledTripStartsTooEarly()
    {
        var assignment = AssignmentWithActiveStudent();
        var scheduled = DateTime.UtcNow.AddMinutes(30);
        var trip = CreateScheduledTrip(assignment, scheduled);
        var handler = CreateHandler(assignment, hasActiveTrip: false, scheduledTrip: trip);

        var act = () => handler.Handle(new StartTripCommand
        {
            TripId = trip.Id,
            RouteAssignmentId = assignment.Id
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("No puede iniciar el viaje antes de la hora programada.");
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Supervisor")]
    public async Task StartTrip_AllowsForcedEarlyStart_ForAdminOrSupervisor(string role)
    {
        var assignment = AssignmentWithActiveStudent();
        var scheduled = DateTime.UtcNow.AddMinutes(30);
        var trip = CreateScheduledTrip(assignment, scheduled);
        var handler = CreateHandler(assignment, hasActiveTrip: false, scheduledTrip: trip, role: role);

        await handler.Handle(new StartTripCommand
        {
            TripId = trip.Id,
            RouteAssignmentId = assignment.Id,
            ForceEarlyStart = true,
            EarlyStartReason = "Salida autorizada"
        }, CancellationToken.None);

        trip.Status.Should().Be(TripStatus.InProgress);
        trip.StartedEarly.Should().BeTrue();
        trip.EarlyStartReason.Should().Be("Salida autorizada");
    }

    [Fact]
    public async Task StartTrip_Fails_WhenForcedEarlyStartHasNoReason()
    {
        var assignment = AssignmentWithActiveStudent();
        var scheduled = DateTime.UtcNow.AddMinutes(30);
        var trip = CreateScheduledTrip(assignment, scheduled);
        var handler = CreateHandler(assignment, hasActiveTrip: false, scheduledTrip: trip, role: "Supervisor");

        var act = () => handler.Handle(new StartTripCommand
        {
            TripId = trip.Id,
            RouteAssignmentId = assignment.Id,
            ForceEarlyStart = true
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task StartTrip_CalculatesDelayMinutes_ForLateScheduledTrip()
    {
        var assignment = AssignmentWithActiveStudent();
        var scheduled = DateTime.UtcNow.AddMinutes(-12);
        var trip = CreateScheduledTrip(assignment, scheduled);
        var handler = CreateHandler(assignment, hasActiveTrip: false, scheduledTrip: trip);

        await handler.Handle(new StartTripCommand
        {
            TripId = trip.Id,
            RouteAssignmentId = assignment.Id
        }, CancellationToken.None);

        trip.DelayMinutes.Should().BeGreaterThanOrEqualTo(12);
        trip.IsLate.Should().BeTrue();
        trip.PunctualityStatus.Should().Be(TripPunctualityStatus.Late);
    }

    private static StartTripHandler CreateHandler(
        RouteAssignment assignment,
        bool hasActiveTrip,
        Trip? scheduledTrip = null,
        string role = "Driver")
    {
        var trips = new Mock<ITripRepository>();
        trips.Setup(x => x.HasActiveTripAsync(assignment.Id)).ReturnsAsync(hasActiveTrip);
        if (scheduledTrip is not null)
            trips.Setup(x => x.GetByIdWithAssignmentDetailsAsync(scheduledTrip.Id)).ReturnsAsync(scheduledTrip);

        return CreateHandler(assignment, trips, role: role);
    }

    private static StartTripHandler CreateHandler(
        RouteAssignment assignment,
        Mock<ITripRepository> trips,
        Mock<ITripStudentAttendanceRepository>? attendances = null,
        string role = "Driver")
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
        var settings = new Mock<ISystemSettingService>();
        var currentUser = new Mock<ICurrentUserService>();
        var operationAuthorization = new Mock<ITripOperationAuthorizationService>();

        assignments.Setup(x => x.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        settings.Setup(x => x.GetIntAsync("TripStartToleranceMinutes", 5)).ReturnsAsync(5);
        currentUser.Setup(x => x.Role).Returns(role);
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
        operationAuthorization.Setup(x => x.EnsureCanStartTripAsync(assignment))
            .Returns(Task.CompletedTask);

        return new StartTripHandler(
            assignments.Object,
            trips.Object,
            attendances.Object,
            audit.Object,
            notifications.Object,
            users.Object,
            settings.Object,
            currentUser.Object,
            operationAuthorization.Object,
            new ImmediateUnitOfWork());
    }

    private static RouteAssignment AssignmentWithActiveStudent()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var student = DomainTestFactory.ActiveStudent(assignment.Route.SchoolId);
        assignment.AssignStudent(student.Id);
        ReflectionHelper.SetProperty(assignment.Students.Single(), nameof(StudentRouteAssignment.Student), student);
        return assignment;
    }

    private static Trip CreateScheduledTrip(RouteAssignment assignment, DateTime scheduled)
    {
        var trip = Trip.CreateScheduledFromSchedule(
            assignment.Id,
            Guid.NewGuid(),
            TripDirection.ToSchool,
            DateOnly.FromDateTime(scheduled),
            scheduled);

        ReflectionHelper.SetProperty(trip, nameof(Trip.RouteAssignment), assignment);
        return trip;
    }
}
