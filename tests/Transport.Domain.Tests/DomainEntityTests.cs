using FluentAssertions;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Domain.Tests;

public class DomainEntityTests
{
    [Fact]
    public void Vehicle_DoesNotAllow_NonPositiveCapacity()
    {
        var act = () => new Vehicle("A123456", 0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Vehicle_CanBeSentToMaintenance_AndDeactivated()
    {
        var vehicle = new Vehicle("A123456", 20);

        vehicle.SendToMaintenance();
        vehicle.Status.Should().Be(VehicleStatus.Maintenance);

        vehicle.Activate();
        vehicle.Deactivate();
        vehicle.Status.Should().Be(VehicleStatus.Inactive);
    }

    [Fact]
    public void Trip_CannotEndBeforeStart()
    {
        var trip = new Trip(Guid.NewGuid());

        var act = () => trip.End();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_StartAndEnd_UpdateStatus()
    {
        var trip = new Trip(Guid.NewGuid());

        trip.Start();
        trip.Status.Should().Be(TripStatus.InProgress);

        trip.End();
        trip.Status.Should().Be(TripStatus.Completed);
    }

    [Fact]
    public void Trip_CannotCancelCompletedTrip()
    {
        var trip = new Trip(Guid.NewGuid());
        trip.Start();
        trip.End();

        var act = () => trip.Cancel();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Route_DoesNotActivateWithoutStops()
    {
        var route = CreateRoute();

        var act = () => route.Activate();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Route_DoesNotAllowDuplicateStopOrder()
    {
        var route = CreateRoute();
        route.AddStop(new RouteStop(route.Id, Guid.NewGuid(), 1));

        var act = () => route.AddStop(new RouteStop(route.Id, Guid.NewGuid(), 1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Route_DoesNotRemoveMissingStop()
    {
        var route = CreateRoute();

        var act = () => route.RemoveStopByStopId(Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RouteAssignment_DoesNotStartTripWithoutStudents()
    {
        var assignment = CreateAssignment(capacity: 1);

        var act = () => assignment.StartTrip();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RouteAssignment_DoesNotAllowDuplicateStudent()
    {
        var assignment = CreateAssignment(capacity: 2);
        var studentId = Guid.NewGuid();
        assignment.AssignStudent(studentId);

        var act = () => assignment.AssignStudent(studentId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RouteAssignment_DoesNotExceedCapacity()
    {
        var assignment = CreateAssignment(capacity: 1);
        assignment.AssignStudent(Guid.NewGuid());

        var act = () => assignment.AssignStudent(Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Notification_CanBeMarkedReadAndUnread()
    {
        var notification = new Notification(
            Guid.NewGuid(),
            "Viaje iniciado",
            "El viaje ha iniciado.",
            NotificationType.TripStarted,
            NotificationPriority.Medium);

        notification.MarkAsRead();
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();

        notification.MarkAsUnread();
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();
    }

    [Fact]
    public void Incident_DoesNotCloseUnlessResolved()
    {
        var incident = CreateIncident();

        var act = () => incident.Close();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Incident_CanResolveOpenIncident()
    {
        var incident = CreateIncident();
        var resolverId = Guid.NewGuid();

        incident.Resolve(resolverId);

        incident.Status.Should().Be(IncidentStatus.Resolved);
        incident.ResolvedByUserId.Should().Be(resolverId);
        incident.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public void Incident_DoesNotResolveClosedOrCancelledIncident()
    {
        var closed = CreateIncident();
        closed.Resolve(Guid.NewGuid());
        closed.Close();

        var cancelled = CreateIncident();
        cancelled.Cancel();

        closed.Invoking(x => x.Resolve(Guid.NewGuid())).Should().Throw<DomainException>();
        cancelled.Invoking(x => x.Resolve(Guid.NewGuid())).Should().Throw<DomainException>();
    }

    private static Route CreateRoute()
    {
        var start = DateTime.UtcNow.Date.AddHours(7);
        return new Route("Ruta Norte", Guid.NewGuid(), new TimeRange(start, start.AddHours(1)));
    }

    private static RouteAssignment CreateAssignment(int capacity)
    {
        return new RouteAssignment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), capacity);
    }

    private static Incident CreateIncident()
    {
        return new Incident(
            "Retraso",
            "El viaje tiene retraso operativo.",
            IncidentType.Delay,
            IncidentSeverity.Medium,
            Guid.NewGuid());
    }
}
