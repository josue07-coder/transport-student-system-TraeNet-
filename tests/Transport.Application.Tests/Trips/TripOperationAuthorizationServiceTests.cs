using FluentAssertions;
using Moq;
using Transport.Application.Common.Security;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Tests.Trips;

public class TripOperationAuthorizationServiceTests
{
    [Fact]
    public async Task AssignedAssistant_CanStartAssignedTrip()
    {
        var assistantId = Guid.NewGuid();
        var assignment = CreateAssignment(assistantId);
        var service = CreateService(CreateUser("TransportAssistant", assistantId: assistantId));

        var act = () => service.EnsureCanStartTripAsync(assignment);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UnassignedAssistant_CannotStartTrip()
    {
        var assignment = CreateAssignment(Guid.NewGuid());
        var service = CreateService(CreateUser("TransportAssistant", assistantId: Guid.NewGuid()));

        var act = () => service.EnsureCanStartTripAsync(assignment);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AssignedAssistant_CanEndAssignedTrip()
    {
        var assistantId = Guid.NewGuid();
        var trip = CreateTrip(CreateAssignment(assistantId));
        var service = CreateService(CreateUser("TransportAssistant", assistantId: assistantId));

        var act = () => service.EnsureCanEndTripAsync(trip);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AssignedAssistant_CanManageAttendanceAndLocationAndDeviation()
    {
        var assistantId = Guid.NewGuid();
        var trip = CreateTrip(CreateAssignment(assistantId));
        var service = CreateService(CreateUser("TransportAssistant", assistantId: assistantId));

        await service.EnsureCanManageTripAttendanceAsync(trip);
        await service.EnsureCanUpdateLocationAsync(trip);
        await service.EnsureCanReportRouteDeviationAsync(trip);
    }

    [Fact]
    public async Task Guardian_CannotOperateTrip()
    {
        var trip = CreateTrip(CreateAssignment(Guid.NewGuid()));
        var service = CreateService(CreateUser("Guardian", guardianId: Guid.NewGuid()));

        var act = () => service.EnsureCanManageTripAttendanceAsync(trip);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Supervisor")]
    public async Task PrivilegedRoles_CanOperateAnyTrip(string roleName)
    {
        var trip = CreateTrip(CreateAssignment(Guid.NewGuid()));
        var service = CreateService(CreateUser(roleName));

        await service.EnsureCanStartTripAsync(trip.RouteAssignment);
        await service.EnsureCanEndTripAsync(trip);
        await service.EnsureCanManageTripAttendanceAsync(trip);
    }

    private static TripOperationAuthorizationService CreateService(User currentUser)
    {
        var visibilityService = new Mock<IVisibilityService>();
        var attendanceRepository = new Mock<ITripStudentAttendanceRepository>();
        visibilityService.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);
        visibilityService.Setup(x => x.EnsureCanViewTripAsync(It.IsAny<Trip>())).Returns(Task.CompletedTask);
        return new TripOperationAuthorizationService(visibilityService.Object, attendanceRepository.Object);
    }

    private static RouteAssignment CreateAssignment(Guid assistantId)
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var assistant = new TransportAssistant(
            DocumentType.Cedula,
            "00100000002",
            "Laura",
            "Gomez",
            PhoneNumber.Create("8095550002"),
            Address.Create("Calle 2", "Santo Domingo"),
            Email.Create("assistant@test.local"));

        ReflectionHelper.SetProperty(assistant, nameof(TransportAssistant.Id), assistantId);
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.TransportAssistantId), assistantId);
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.TransportAssistant), assistant);
        return assignment;
    }

    private static Trip CreateTrip(RouteAssignment assignment)
    {
        var trip = new Trip(assignment.Id);
        ReflectionHelper.SetProperty(trip, nameof(Trip.RouteAssignment), assignment);
        return trip;
    }

    private static User CreateUser(
        string roleName,
        Guid? guardianId = null,
        Guid? driverId = null,
        Guid? assistantId = null)
    {
        var role = new Role(roleName, roleName);
        var user = new User(
            $"{roleName}-{Guid.NewGuid():N}",
            $"{roleName} User",
            $"{Guid.NewGuid():N}@test.local",
            "hash",
            role.Id,
            guardianId,
            driverId,
            assistantId);

        ReflectionHelper.SetProperty(user, nameof(User.Role), role);
        return user;
    }
}
