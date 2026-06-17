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

        trip.Status.Should().Be(TripStatus.Scheduled);

        trip.Start();
        trip.Status.Should().Be(TripStatus.InProgress);

        trip.End();
        trip.Status.Should().Be(TripStatus.Completed);
    }

    [Fact]
    public void Trip_CreateScheduledFromSchedule_CreatesScheduledTrip()
    {
        var routeAssignmentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var operationDate = new DateOnly(2026, 6, 8);
        var departure = operationDate.ToDateTime(new TimeOnly(6, 30));
        var arrival = operationDate.ToDateTime(new TimeOnly(7, 30));

        var trip = Trip.CreateScheduledFromSchedule(
            routeAssignmentId,
            scheduleId,
            TripDirection.ToSchool,
            operationDate,
            departure,
            arrival);

        trip.Status.Should().Be(TripStatus.Scheduled);
        trip.RouteAssignmentId.Should().Be(routeAssignmentId);
        trip.TripScheduleId.Should().Be(scheduleId);
        trip.Direction.Should().Be(TripDirection.ToSchool);
        trip.OperationDate.Should().Be(operationDate);
        trip.ScheduledDepartureTime.Should().Be(departure);
        trip.ScheduledArrivalTime.Should().Be(arrival);
        trip.StartTime.Should().BeNull();
        trip.EndTime.Should().BeNull();
    }

    [Fact]
    public void Trip_CreateScheduledFromSchedule_CanStartEndAndCancel()
    {
        var operationDate = new DateOnly(2026, 6, 8);
        var trip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.FromSchool,
            operationDate,
            operationDate.ToDateTime(new TimeOnly(13, 30)));

        trip.Cancel("Cancelación programada");
        trip.Status.Should().Be(TripStatus.Cancelled);

        var secondTrip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.FromSchool,
            operationDate,
            operationDate.ToDateTime(new TimeOnly(13, 30)));

        secondTrip.Start();
        secondTrip.Status.Should().Be(TripStatus.InProgress);

        secondTrip.End();
        secondTrip.Status.Should().Be(TripStatus.Completed);
    }

    [Fact]
    public void Trip_Cancel_RequiresReason()
    {
        var trip = new Trip(Guid.NewGuid());

        var act = () => trip.Cancel(" ");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_Cancel_ChangesScheduledToCancelled()
    {
        var trip = new Trip(Guid.NewGuid());

        trip.Cancel("Clases suspendidas");

        trip.Status.Should().Be(TripStatus.Cancelled);
        trip.CancellationReason.Should().Be("Clases suspendidas");
    }

    [Fact]
    public void Trip_CannotCancelInProgressTrip()
    {
        var trip = new Trip(Guid.NewGuid());
        trip.Start();

        var act = () => trip.Cancel("Clases suspendidas");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_CannotStartCompletedCancelledOrNotOperatingTrip()
    {
        var completed = new Trip(Guid.NewGuid());
        completed.Start();
        completed.End();

        var cancelled = new Trip(Guid.NewGuid());
        cancelled.Cancel("Clases suspendidas");

        var notOperating = new Trip(Guid.NewGuid());
        notOperating.MarkNotOperating("Feriado", null);

        completed.Invoking(x => x.Start()).Should().Throw<DomainException>();
        cancelled.Invoking(x => x.Start()).Should().Throw<DomainException>();
        notOperating.Invoking(x => x.Start()).Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_MarkNotOperating_RequiresReason()
    {
        var trip = new Trip(Guid.NewGuid());

        var act = () => trip.MarkNotOperating("", null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_MarkNotOperating_ChangesScheduledToNotOperating()
    {
        var trip = new Trip(Guid.NewGuid());

        trip.MarkNotOperating("Feriado", "Día no lectivo");

        trip.Status.Should().Be(TripStatus.NotOperating);
        trip.NonOperationReason.Should().Be("Feriado");
        trip.NonOperationNotes.Should().Be("Día no lectivo");
    }

    [Fact]
    public void Trip_CannotMarkNotOperatingWhenInProgress()
    {
        var trip = new Trip(Guid.NewGuid());
        trip.Start();

        var act = () => trip.MarkNotOperating("Feriado", null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TripSchedule_DefaultsToWeekdaysAndActive()
    {
        var schedule = new TripSchedule(
            Guid.NewGuid(),
            TripDirection.ToSchool,
            new TimeOnly(6, 30),
            new TimeOnly(7, 30),
            DateOnly.FromDateTime(DateTime.UtcNow));

        schedule.IsActive.Should().BeTrue();
        schedule.Monday.Should().BeTrue();
        schedule.Tuesday.Should().BeTrue();
        schedule.Wednesday.Should().BeTrue();
        schedule.Thursday.Should().BeTrue();
        schedule.Friday.Should().BeTrue();
        schedule.Saturday.Should().BeFalse();
        schedule.Sunday.Should().BeFalse();
    }

    [Fact]
    public void TripSchedule_DoesNotAllowNoActiveDays()
    {
        var act = () => new TripSchedule(
            Guid.NewGuid(),
            TripDirection.ToSchool,
            new TimeOnly(6, 30),
            null,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            false,
            false,
            false,
            false,
            false,
            false,
            false);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TripSchedule_DoesNotAllowArrivalBeforeOrEqualDeparture()
    {
        var act = () => new TripSchedule(
            Guid.NewGuid(),
            TripDirection.FromSchool,
            new TimeOnly(13, 30),
            new TimeOnly(13, 30),
            DateOnly.FromDateTime(DateTime.UtcNow));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TripSchedule_DoesNotAllowValidToBeforeValidFrom()
    {
        var validFrom = DateOnly.FromDateTime(DateTime.UtcNow);

        var act = () => new TripSchedule(
            Guid.NewGuid(),
            TripDirection.FromSchool,
            new TimeOnly(13, 30),
            new TimeOnly(14, 30),
            validFrom,
            validFrom.AddDays(-1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TripSchedule_CanActivateAndDeactivate()
    {
        var schedule = new TripSchedule(
            Guid.NewGuid(),
            TripDirection.ToSchool,
            new TimeOnly(6, 30),
            null,
            DateOnly.FromDateTime(DateTime.UtcNow));

        schedule.Deactivate();
        schedule.IsActive.Should().BeFalse();

        schedule.Activate();
        schedule.IsActive.Should().BeTrue();
    }

    [Fact]
    public void NonSchoolDay_CanBeCreated()
    {
        var day = new NonSchoolDay(
            new DateOnly(2026, 6, 8),
            NonSchoolDayReason.Holiday,
            "Feriado nacional");

        day.IsActive.Should().BeTrue();
        day.Date.Should().Be(new DateOnly(2026, 6, 8));
        day.SchoolId.Should().BeNull();
        day.ReasonType.Should().Be(NonSchoolDayReason.Holiday);
        day.Reason.Should().Be("Feriado nacional");
    }

    [Fact]
    public void NonSchoolDay_DoesNotAllowEmptyReason()
    {
        var act = () => new NonSchoolDay(
            new DateOnly(2026, 6, 8),
            NonSchoolDayReason.Holiday,
            " ");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void NonSchoolDay_CanBeDeactivated()
    {
        var day = new NonSchoolDay(
            new DateOnly(2026, 6, 8),
            NonSchoolDayReason.Weather,
            "Lluvias intensas");

        day.Deactivate();

        day.IsActive.Should().BeFalse();
    }

    [Fact]
    public void TripStudentAttendance_StartsExpected()
    {
        var attendance = CreateAttendance();

        attendance.Status.Should().Be(TripAttendanceStatus.Expected);
        attendance.BoardedAt.Should().BeNull();
        attendance.DroppedOffAt.Should().BeNull();
    }

    [Fact]
    public void TripStudentAttendance_CanBeMarkedBoardedAbsentAndDroppedOff()
    {
        var boarded = CreateAttendance();
        var boardedAt = DateTime.UtcNow;
        boarded.MarkBoarded(boardedAt);
        boarded.Status.Should().Be(TripAttendanceStatus.Boarded);
        boarded.BoardedAt.Should().Be(boardedAt);

        var absent = CreateAttendance();
        absent.MarkAbsent("No asistió");
        absent.Status.Should().Be(TripAttendanceStatus.Absent);
        absent.Notes.Should().Be("No asistió");

        var droppedOff = CreateAttendance();
        var droppedOffAt = DateTime.UtcNow;
        droppedOff.MarkBoarded(DateTime.UtcNow);
        droppedOff.MarkDroppedOff(droppedOffAt);
        droppedOff.Status.Should().Be(TripAttendanceStatus.DroppedOff);
        droppedOff.DroppedOffAt.Should().Be(droppedOffAt);
    }

    [Fact]
    public void TripStudentAttendance_DoesNotAllowDroppedOffBeforeBoarded()
    {
        var attendance = CreateAttendance();

        var act = () => attendance.MarkDroppedOff(DateTime.UtcNow);

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

    private static TripStudentAttendance CreateAttendance()
    {
        return new TripStudentAttendance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Ana Santos",
            "STU-001",
            Guid.NewGuid(),
            "Maria Santos");
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
