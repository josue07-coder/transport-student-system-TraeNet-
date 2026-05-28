using FluentAssertions;
using Moq;
using Transport.Application.Features.Trips.Commands.StartTrip;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Trips;

public class StartTripHandlerTests
{
    [Fact]
    public async Task StartTrip_Fails_WhenTripAlreadyActive()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        assignment.AssignStudent(Guid.NewGuid());
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

    private static StartTripHandler CreateHandler(RouteAssignment assignment, bool hasActiveTrip)
    {
        var assignments = new Mock<IRouteAssignmentRepository>();
        var trips = new Mock<ITripRepository>();
        var audit = new Mock<IAuditService>();
        var notifications = new Mock<INotificationService>();
        var users = new Mock<IUserRepository>();

        assignments.Setup(x => x.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        trips.Setup(x => x.HasActiveTripAsync(assignment.Id)).ReturnsAsync(hasActiveTrip);

        return new StartTripHandler(assignments.Object, trips.Object, audit.Object, notifications.Object, users.Object);
    }
}
