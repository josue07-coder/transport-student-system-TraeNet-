using FluentAssertions;
using Moq;
using Transport.Application.Features.TripStudentAttendances.Commands.AddExceptionalPassenger;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Tests.Trips;

public class AddExceptionalPassengerHandlerTests
{
    [Fact]
    public async Task AddExceptionalPassenger_Fails_WhenTripDoesNotExist()
    {
        var context = CreateContext(null, DomainTestFactory.ActiveStudent(Guid.NewGuid()), CreateUser("Admin"));
        var handler = CreateHandler(context);

        var act = () => handler.Handle(CreateCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AddExceptionalPassenger_Fails_WhenTripIsNotInProgress()
    {
        var trip = CreateTrip(inProgress: false);
        var student = DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var context = CreateContext(trip, student, CreateUser("Admin"));
        var handler = CreateHandler(context);

        var act = () => handler.Handle(CreateCommand(trip.Id, student.Id), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AddExceptionalPassenger_Fails_WhenStudentDoesNotExist()
    {
        var trip = CreateTrip();
        var context = CreateContext(trip, null, CreateUser("Admin"));
        var handler = CreateHandler(context);

        var act = () => handler.Handle(CreateCommand(trip.Id, Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AddExceptionalPassenger_Fails_WhenStudentAlreadyExistsInTrip()
    {
        var trip = CreateTrip();
        var student = DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var context = CreateContext(trip, student, CreateUser("Admin"), existsAttendance: true);
        var handler = CreateHandler(context);

        var act = () => handler.Handle(CreateCommand(trip.Id, student.Id), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AddExceptionalPassenger_Fails_WhenStudentBelongsToOfficialAssignment()
    {
        var trip = CreateTrip();
        var student = DomainTestFactory.ActiveStudent(trip.RouteAssignment.Route.SchoolId);
        trip.RouteAssignment.AssignStudent(student.Id);
        ReflectionHelper.SetProperty(trip.RouteAssignment.Students.Single(), nameof(StudentRouteAssignment.Student), student);
        var context = CreateContext(trip, student, CreateUser("Admin"));
        var handler = CreateHandler(context);

        var act = () => handler.Handle(CreateCommand(trip.Id, student.Id), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("El estudiante pertenece a la asignación oficial*");
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Supervisor")]
    public async Task AddExceptionalPassenger_AllowsAdminAndSupervisor(string role)
    {
        var trip = CreateTrip();
        var student = DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var context = CreateContext(trip, student, CreateUser(role));
        var handler = CreateHandler(context);

        var result = await handler.Handle(CreateCommand(trip.Id, student.Id), CancellationToken.None);

        result.IsExpectedPassenger.Should().BeFalse();
        result.Status.Should().Be(TripAttendanceStatus.Boarded);
        result.ExceptionReason.Should().Be("Cambio temporal autorizado");
        context.SavedAttendance.Should().NotBeNull();
    }

    [Fact]
    public async Task AddExceptionalPassenger_AllowsAssignedDriver()
    {
        var trip = CreateTrip();
        var student = DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var context = CreateContext(trip, student, CreateUser("Driver", driverId: trip.RouteAssignment.DriverId));
        var handler = CreateHandler(context);

        var result = await handler.Handle(CreateCommand(trip.Id, student.Id), CancellationToken.None);

        result.IsExpectedPassenger.Should().BeFalse();
        context.SavedAttendance.Should().NotBeNull();
    }

    [Fact]
    public async Task AddExceptionalPassenger_AllowsAssignedAssistant()
    {
        var assistantId = Guid.NewGuid();
        var trip = CreateTrip(assistantId: assistantId);
        var student = DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var context = CreateContext(trip, student, CreateUser("TransportAssistant", assistantId: assistantId));
        var handler = CreateHandler(context);

        var result = await handler.Handle(CreateCommand(trip.Id, student.Id), CancellationToken.None);

        result.IsExpectedPassenger.Should().BeFalse();
        context.SavedAttendance.Should().NotBeNull();
    }

    [Fact]
    public async Task AddExceptionalPassenger_BlocksGuardian()
    {
        var trip = CreateTrip();
        var student = DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var context = CreateContext(trip, student, CreateUser("Guardian"));
        var handler = CreateHandler(context);

        var act = () => handler.Handle(CreateCommand(trip.Id, student.Id), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    private static AddExceptionalPassengerCommand CreateCommand(Guid tripId, Guid studentId)
    {
        return new AddExceptionalPassengerCommand
        {
            TripId = tripId,
            StudentId = studentId,
            ExceptionReason = "Cambio temporal autorizado",
            Notes = "Tutor informado"
        };
    }

    private static AddExceptionalPassengerHandler CreateHandler(TestContext context)
    {
        return new AddExceptionalPassengerHandler(
            context.TripRepository.Object,
            context.AttendanceRepository.Object,
            context.StudentRepository.Object,
            context.UserRepository.Object,
            context.CurrentUserService.Object,
            context.AuditService.Object,
            context.NotificationService.Object,
            context.OperationAuthorizationService.Object,
            new ImmediateUnitOfWork());
    }

    private static TestContext CreateContext(Trip? trip, Student? student, User currentUser, bool existsAttendance = false)
    {
        var context = new TestContext();

        if (trip is not null)
            context.TripRepository.Setup(x => x.GetByIdWithAssignmentDetailsAsync(trip.Id)).ReturnsAsync(trip);

        if (student is not null)
            context.StudentRepository.Setup(x => x.GetByIdAsync(student.Id)).ReturnsAsync(student);

        context.UserRepository.Setup(x => x.GetByIdWithLinkedProfilesAsync(currentUser.Id)).ReturnsAsync(currentUser);
        context.CurrentUserService.Setup(x => x.UserId).Returns(currentUser.Id);
        context.AttendanceRepository.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(existsAttendance);
        context.AttendanceRepository.Setup(x => x.AddAsync(It.IsAny<TripStudentAttendance>()))
            .Callback<TripStudentAttendance>(attendance => context.SavedAttendance = attendance)
            .Returns(Task.CompletedTask);
        context.AttendanceRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        context.AttendanceRepository.Setup(x => x.GetByTripAndStudentAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => context.SavedAttendance);
        context.AuditService.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        context.NotificationService.Setup(x => x.NotifyRoleAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<NotificationType>(),
                It.IsAny<NotificationPriority>(),
                It.IsAny<string?>(),
                It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        context.OperationAuthorizationService.Setup(x => x.EnsureCanManageTripAttendanceAsync(It.IsAny<Trip>()))
            .Callback(() =>
            {
                if (string.Equals(currentUser.Role?.Name, "Guardian", StringComparison.OrdinalIgnoreCase))
                    throw new DomainException("No tiene permiso para actualizar pasajeros de este viaje");
            })
            .Returns(Task.CompletedTask);

        return context;
    }

    private static Trip CreateTrip(bool inProgress = true, Guid? assistantId = null)
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        if (assistantId.HasValue)
        {
            var assistant = new TransportAssistant(
                DocumentType.Cedula,
                "00100000002",
                "Laura",
                "Gomez",
                PhoneNumber.Create("8095550002"),
                Address.Create("Calle 2", "Santo Domingo"),
                Email.Create("assistant@test.local"));
            ReflectionHelper.SetProperty(assistant, nameof(TransportAssistant.Id), assistantId.Value);
            ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.TransportAssistantId), assistantId.Value);
            ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.TransportAssistant), assistant);
        }

        var trip = new Trip(assignment.Id);
        ReflectionHelper.SetProperty(trip, nameof(Trip.RouteAssignment), assignment);
        if (inProgress)
            trip.Start();
        return trip;
    }

    private static User CreateUser(string roleName, Guid? driverId = null, Guid? assistantId = null)
    {
        var role = new Role(roleName, roleName);
        var user = new User(
            $"{roleName}-{Guid.NewGuid():N}",
            $"{roleName} User",
            $"{Guid.NewGuid():N}@test.local",
            "hash",
            role.Id,
            null,
            driverId,
            assistantId);

        ReflectionHelper.SetProperty(user, nameof(User.Role), role);
        return user;
    }

    private sealed class TestContext
    {
        public Mock<ITripRepository> TripRepository { get; } = new();
        public Mock<ITripStudentAttendanceRepository> AttendanceRepository { get; } = new();
        public Mock<IStudentRepository> StudentRepository { get; } = new();
        public Mock<IUserRepository> UserRepository { get; } = new();
        public Mock<ICurrentUserService> CurrentUserService { get; } = new();
        public Mock<IAuditService> AuditService { get; } = new();
        public Mock<INotificationService> NotificationService { get; } = new();
        public Mock<ITripOperationAuthorizationService> OperationAuthorizationService { get; } = new();
        public TripStudentAttendance? SavedAttendance { get; set; }
    }
}
