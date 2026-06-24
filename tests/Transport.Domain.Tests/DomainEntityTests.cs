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
    public void Trip_Start_OnTimeWithinTolerance_DoesNotMarkLate()
    {
        var scheduled = DateTime.UtcNow.AddMinutes(5);
        var trip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.ToSchool,
            DateOnly.FromDateTime(scheduled),
            scheduled);

        trip.Start(scheduled.AddMinutes(-5), 5, false, null);

        trip.Status.Should().Be(TripStatus.InProgress);
        trip.DelayMinutes.Should().Be(0);
        trip.IsLate.Should().BeFalse();
        trip.StartedEarly.Should().BeFalse();
        trip.PunctualityStatus.Should().Be(TripPunctualityStatus.OnTime);
    }

    [Fact]
    public void Trip_Start_DoesNotAllowTooEarlyWithoutAuthorization()
    {
        var scheduled = DateTime.UtcNow.AddMinutes(30);
        var trip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.ToSchool,
            DateOnly.FromDateTime(scheduled),
            scheduled);

        var act = () => trip.Start(scheduled.AddMinutes(-6), 5, false, null);

        act.Should().Throw<DomainException>()
            .WithMessage("No puede iniciar el viaje antes de la hora programada.");
    }

    [Fact]
    public void Trip_Start_ForcedEarlyRequiresReason()
    {
        var scheduled = DateTime.UtcNow.AddMinutes(30);
        var trip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.ToSchool,
            DateOnly.FromDateTime(scheduled),
            scheduled);

        var act = () => trip.Start(scheduled.AddMinutes(-10), 5, true, " ");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_Start_ForcedEarlyStoresReason()
    {
        var scheduled = DateTime.UtcNow.AddMinutes(30);
        var trip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.ToSchool,
            DateOnly.FromDateTime(scheduled),
            scheduled);

        trip.Start(scheduled.AddMinutes(-10), 5, true, "Autorizado por supervisor");

        trip.StartedEarly.Should().BeTrue();
        trip.EarlyStartReason.Should().Be("Autorizado por supervisor");
        trip.DelayMinutes.Should().Be(0);
        trip.PunctualityStatus.Should().Be(TripPunctualityStatus.OnTime);
    }

    [Fact]
    public void Trip_Start_CalculatesSlightDelay()
    {
        var scheduled = DateTime.UtcNow.AddMinutes(-7);
        var trip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.ToSchool,
            DateOnly.FromDateTime(scheduled),
            scheduled);

        trip.Start(scheduled.AddMinutes(7), 5, false, null);

        trip.DelayMinutes.Should().Be(7);
        trip.IsLate.Should().BeTrue();
        trip.PunctualityStatus.Should().Be(TripPunctualityStatus.SlightlyLate);
    }

    [Fact]
    public void Trip_Start_CalculatesLateDelay()
    {
        var scheduled = DateTime.UtcNow.AddMinutes(-12);
        var trip = Trip.CreateScheduledFromSchedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TripDirection.ToSchool,
            DateOnly.FromDateTime(scheduled),
            scheduled);

        trip.Start(scheduled.AddMinutes(12), 5, false, null);

        trip.DelayMinutes.Should().Be(12);
        trip.IsLate.Should().BeTrue();
        trip.PunctualityStatus.Should().Be(TripPunctualityStatus.Late);
    }

    [Fact]
    public void Trip_ReportRouteDeviation_DoesNotAllowWhenNotInProgress()
    {
        var trip = new Trip(Guid.NewGuid());

        var act = () => trip.ReportRouteDeviation(
            Guid.NewGuid(),
            RouteDeviationReasonType.Traffic,
            "Tránsito pesado",
            null,
            null,
            null,
            DateTime.UtcNow);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_ReportRouteDeviation_AllowsWhenInProgress()
    {
        var trip = new Trip(Guid.NewGuid());
        trip.Start();
        var userId = Guid.NewGuid();

        var deviation = trip.ReportRouteDeviation(
            userId,
            RouteDeviationReasonType.RoadClosed,
            "Puente cerrado",
            "Se tomó una vía alterna",
            18.208100m,
            -71.100200m,
            DateTime.UtcNow);

        deviation.TripId.Should().Be(trip.Id);
        deviation.ReportedByUserId.Should().Be(userId);
        deviation.ReasonType.Should().Be(RouteDeviationReasonType.RoadClosed);
        deviation.Reason.Should().Be("Puente cerrado");
        deviation.Latitude.Should().Be(18.208100m);
        deviation.Longitude.Should().Be(-71.100200m);
        trip.RouteDeviations.Should().ContainSingle();
    }

    [Fact]
    public void Trip_ReportRouteDeviation_OtherRequiresReason()
    {
        var trip = new Trip(Guid.NewGuid());
        trip.Start();

        var act = () => trip.ReportRouteDeviation(
            Guid.NewGuid(),
            RouteDeviationReasonType.Other,
            " ",
            null,
            null,
            null,
            DateTime.UtcNow);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_ReportRouteDeviation_AllowsOptionalCoordinates()
    {
        var trip = new Trip(Guid.NewGuid());
        trip.Start();

        var deviation = trip.ReportRouteDeviation(
            Guid.NewGuid(),
            RouteDeviationReasonType.Weather,
            "Lluvia fuerte",
            null,
            null,
            null,
            DateTime.UtcNow);

        deviation.Latitude.Should().BeNull();
        deviation.Longitude.Should().BeNull();
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
        attendance.IsExpectedPassenger.Should().BeTrue();
        attendance.AttendanceSource.Should().Be(AttendanceSource.Manual);
        attendance.MarkedAt.Should().BeNull();
        attendance.MarkedByUserId.Should().BeNull();
        attendance.BoardedAt.Should().BeNull();
        attendance.DroppedOffAt.Should().BeNull();
    }

    [Fact]
    public void TripStudentAttendance_ExceptionalPassengerRequiresReason()
    {
        var act = () => TripStudentAttendance.CreateExceptionalPassenger(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Ana Santos",
            "STU-001",
            Guid.NewGuid(),
            "Maria Santos",
            " ",
            Guid.NewGuid(),
            DateTime.UtcNow,
            null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TripStudentAttendance_ExceptionalPassengerStartsBoarded()
    {
        var boardedAt = DateTime.UtcNow;
        var registeredByUserId = Guid.NewGuid();

        var attendance = TripStudentAttendance.CreateExceptionalPassenger(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Ana Santos",
            "STU-001",
            Guid.NewGuid(),
            "Maria Santos",
            "Cambio temporal autorizado",
            registeredByUserId,
            boardedAt,
            "Tutor informado");

        attendance.IsExpectedPassenger.Should().BeFalse();
        attendance.Status.Should().Be(TripAttendanceStatus.Boarded);
        attendance.BoardedAt.Should().Be(boardedAt);
        attendance.MarkedAt.Should().Be(boardedAt);
        attendance.MarkedByUserId.Should().Be(registeredByUserId);
        attendance.AttendanceSource.Should().Be(AttendanceSource.ExceptionalManual);
        attendance.ExceptionReason.Should().Be("Cambio temporal autorizado");
        attendance.RegisteredByUserId.Should().Be(registeredByUserId);
        attendance.RegisteredAt.Should().NotBeNull();
        attendance.Notes.Should().Be("Tutor informado");
    }

    [Fact]
    public void TripStudentAttendance_MarkBoardedStoresMarkerAndAutomaticTime()
    {
        var attendance = CreateAttendance();
        var markedByUserId = Guid.NewGuid();
        var boardedAt = DateTime.UtcNow;

        attendance.MarkBoarded(boardedAt, markedByUserId);

        attendance.Status.Should().Be(TripAttendanceStatus.Boarded);
        attendance.BoardedAt.Should().Be(boardedAt);
        attendance.MarkedAt.Should().Be(boardedAt);
        attendance.MarkedByUserId.Should().Be(markedByUserId);
        attendance.AttendanceSource.Should().Be(AttendanceSource.Manual);
    }

    [Fact]
    public void TripStudentAttendance_ExceptionalPassengerRequiresBoardedAt()
    {
        var act = () => TripStudentAttendance.CreateExceptionalPassenger(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Ana Santos",
            "STU-001",
            Guid.NewGuid(),
            "Maria Santos",
            "Cambio temporal autorizado",
            Guid.NewGuid(),
            default,
            null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Trip_AddExceptionalPassenger_RequiresInProgressTrip()
    {
        var trip = new Trip(Guid.NewGuid());

        var act = () => trip.AddExceptionalPassenger(
            Guid.NewGuid(),
            "Ana Santos",
            "STU-001",
            Guid.NewGuid(),
            "Maria Santos",
            "Cambio temporal autorizado",
            Guid.NewGuid(),
            DateTime.UtcNow,
            null);

        act.Should().Throw<DomainException>();
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
    public void TripStudentAttendance_DoesNotAllowAbsentAfterBoarded()
    {
        var attendance = CreateAttendance();
        attendance.MarkBoarded(DateTime.UtcNow);

        var act = () => attendance.MarkAbsent("No asistió");

        act.Should().Throw<DomainException>()
            .WithMessage("No se puede marcar ausente un estudiante que ya abordó");
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
