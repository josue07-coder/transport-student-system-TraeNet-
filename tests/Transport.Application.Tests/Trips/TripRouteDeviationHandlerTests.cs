using FluentAssertions;
using Moq;
using Transport.Application.Features.TripRouteDeviations.Commands.ReportTripRouteDeviation;
using Transport.Application.Features.TripRouteDeviations.Queries.GetTripRouteDeviations;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Trips;

public class TripRouteDeviationHandlerTests
{
    [Fact]
    public async Task ReportDeviation_Fails_WhenTripDoesNotExist()
    {
        var context = CreateContext(null, CreateUser("Admin"));
        var handler = CreateReportHandler(context);

        var act = () => handler.Handle(new ReportTripRouteDeviationCommand
        {
            TripId = Guid.NewGuid(),
            ReasonType = RouteDeviationReasonType.Traffic
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task ReportDeviation_Fails_WhenTripIsCompleted()
    {
        var trip = CreateInProgressTrip();
        trip.End();
        var context = CreateContext(trip, CreateUser("Admin"));
        var handler = CreateReportHandler(context);

        var act = () => handler.Handle(new ReportTripRouteDeviationCommand
        {
            TripId = trip.Id,
            ReasonType = RouteDeviationReasonType.Traffic
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task ReportDeviation_Fails_WhenUserHasNoPermission()
    {
        var trip = CreateInProgressTrip();
        var context = CreateContext(trip, CreateUser("Guardian"));
        var handler = CreateReportHandler(context);

        var act = () => handler.Handle(new ReportTripRouteDeviationCommand
        {
            TripId = trip.Id,
            ReasonType = RouteDeviationReasonType.Traffic
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task ReportDeviation_AllowsAssignedDriver()
    {
        var trip = CreateInProgressTrip();
        var user = CreateUser("Driver", driverId: trip.RouteAssignment.DriverId);
        var context = CreateContext(trip, user);
        var handler = CreateReportHandler(context);

        var result = await handler.Handle(new ReportTripRouteDeviationCommand
        {
            TripId = trip.Id,
            ReasonType = RouteDeviationReasonType.RoadClosed,
            Reason = "Puente cerrado",
            Latitude = 18.208100m,
            Longitude = -71.100200m
        }, CancellationToken.None);

        result.TripId.Should().Be(trip.Id);
        result.ReportedByUserId.Should().Be(user.Id);
        result.ReasonType.Should().Be(RouteDeviationReasonType.RoadClosed);
        result.Reason.Should().Be("Puente cerrado");
        context.SavedDeviations.Should().ContainSingle();
    }

    [Fact]
    public async Task ReportDeviation_AllowsAssignedAssistant()
    {
        var assistantId = Guid.NewGuid();
        var trip = CreateInProgressTrip(assistantId);
        var user = CreateUser("TransportAssistant", assistantId: assistantId);
        var context = CreateContext(trip, user);
        var handler = CreateReportHandler(context);

        var result = await handler.Handle(new ReportTripRouteDeviationCommand
        {
            TripId = trip.Id,
            ReasonType = RouteDeviationReasonType.Weather,
            Reason = "Lluvia fuerte"
        }, CancellationToken.None);

        result.ReportedByUserId.Should().Be(user.Id);
        context.SavedDeviations.Should().ContainSingle();
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Supervisor")]
    public async Task ReportDeviation_AllowsAdminAndSupervisor(string role)
    {
        var trip = CreateInProgressTrip();
        var context = CreateContext(trip, CreateUser(role));
        var handler = CreateReportHandler(context);

        await handler.Handle(new ReportTripRouteDeviationCommand
        {
            TripId = trip.Id,
            ReasonType = RouteDeviationReasonType.Security,
            Reason = "Zona bloqueada"
        }, CancellationToken.None);

        context.SavedDeviations.Should().ContainSingle();
    }

    [Fact]
    public async Task GetTripRouteDeviations_ReturnsDtos()
    {
        var trip = CreateInProgressTrip();
        var user = CreateUser("Admin");
        var deviation = trip.ReportRouteDeviation(
            user.Id,
            RouteDeviationReasonType.Accident,
            "Accidente en vía",
            null,
            null,
            null,
            DateTime.UtcNow);
        var context = CreateContext(trip, user);
        context.ExistingDeviations.Add(deviation);
        var handler = new GetTripRouteDeviationsHandler(
            context.TripRepository.Object,
            context.DeviationRepository.Object,
            context.OperationAuthorizationService.Object);

        var result = await handler.Handle(new GetTripRouteDeviationsQuery(trip.Id), CancellationToken.None);

        result.Should().ContainSingle();
        result.Single().ReasonType.Should().Be(RouteDeviationReasonType.Accident);
    }

    private static ReportTripRouteDeviationHandler CreateReportHandler(TestContext context)
    {
        return new ReportTripRouteDeviationHandler(
            context.TripRepository.Object,
            context.DeviationRepository.Object,
            context.CurrentUserService.Object,
            context.AuditService.Object,
            context.NotificationService.Object,
            context.OperationAuthorizationService.Object,
            new ImmediateUnitOfWork());
    }

    private static TestContext CreateContext(Trip? trip, User currentUser)
    {
        var context = new TestContext();

        if (trip is not null)
            context.TripRepository.Setup(x => x.GetByIdWithAssignmentDetailsAsync(trip.Id)).ReturnsAsync(trip);

        context.UserRepository.Setup(x => x.GetByIdWithLinkedProfilesAsync(currentUser.Id)).ReturnsAsync(currentUser);
        context.CurrentUserService.Setup(x => x.UserId).Returns(currentUser.Id);
        context.DeviationRepository.Setup(x => x.AddAsync(It.IsAny<TripRouteDeviation>()))
            .Callback<TripRouteDeviation>(deviation => context.SavedDeviations.Add(deviation))
            .Returns(Task.CompletedTask);
        context.DeviationRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        context.DeviationRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => context.SavedDeviations.Concat(context.ExistingDeviations).FirstOrDefault(x => x.Id == id));
        if (trip is not null)
            context.DeviationRepository.Setup(x => x.GetByTripAsync(trip.Id)).ReturnsAsync(context.ExistingDeviations);

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
        context.OperationAuthorizationService.Setup(x => x.EnsureCanReportRouteDeviationAsync(It.IsAny<Trip>()))
            .Callback<Trip>(tripToAuthorize =>
            {
                var user = currentUser;
                var assignment = tripToAuthorize.RouteAssignment;
                var isPrivileged = string.Equals(user.Role?.Name, "Admin", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(user.Role?.Name, "Supervisor", StringComparison.OrdinalIgnoreCase);
                var isAssignedDriver = string.Equals(user.Role?.Name, "Driver", StringComparison.OrdinalIgnoreCase)
                    && user.DriverId.HasValue
                    && assignment.DriverId == user.DriverId.Value;
                var isAssignedAssistant = string.Equals(user.Role?.Name, "TransportAssistant", StringComparison.OrdinalIgnoreCase)
                    && user.TransportAssistantId.HasValue
                    && assignment.TransportAssistantId == user.TransportAssistantId.Value;

                if (!isPrivileged && !isAssignedDriver && !isAssignedAssistant)
                    throw new DomainException("No tiene permiso para gestionar desvíos de este viaje");
            })
            .Returns(Task.CompletedTask);

        return context;
    }

    private static Trip CreateInProgressTrip(Guid? assistantId = null)
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        if (assistantId.HasValue)
        {
            var assistant = new TransportAssistant(
                DocumentType.Cedula,
                "00100000002",
                "Laura",
                "Gomez",
                Domain.ValueObjects.PhoneNumber.Create("8095550002"),
                Address.Create("Calle 2", "Santo Domingo"),
                Domain.ValueObjects.Email.Create("assistant@test.local"));
            ReflectionHelper.SetProperty(assistant, nameof(TransportAssistant.Id), assistantId.Value);
            ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.TransportAssistantId), assistantId.Value);
            ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.TransportAssistant), assistant);
        }

        var trip = new Trip(assignment.Id);
        ReflectionHelper.SetProperty(trip, nameof(Trip.RouteAssignment), assignment);
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
        public Mock<ITripRouteDeviationRepository> DeviationRepository { get; } = new();
        public Mock<IUserRepository> UserRepository { get; } = new();
        public Mock<ICurrentUserService> CurrentUserService { get; } = new();
        public Mock<IAuditService> AuditService { get; } = new();
        public Mock<INotificationService> NotificationService { get; } = new();
        public Mock<ITripOperationAuthorizationService> OperationAuthorizationService { get; } = new();
        public List<TripRouteDeviation> SavedDeviations { get; } = new();
        public List<TripRouteDeviation> ExistingDeviations { get; } = new();
    }
}
