using FluentAssertions;
using Moq;
using Transport.Application.Features.TripSchedules.Commands.CreateTripSchedule;
using Transport.Application.Features.TripSchedules.Commands.DeleteTripSchedule;
using Transport.Application.Features.TripSchedules.Commands.MaterializeTripSchedule;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.TripSchedules;

public class TripScheduleHandlerTests
{
    [Fact]
    public async Task CreateTripSchedule_Fails_WhenAssignmentDoesNotExist()
    {
        var schedules = new Mock<ITripScheduleRepository>();
        var assignments = new Mock<IRouteAssignmentRepository>();
        var audit = new Mock<IAuditService>();
        var handler = new CreateTripScheduleHandler(schedules.Object, assignments.Object, audit.Object);

        var act = () => handler.Handle(CreateCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task CreateTripSchedule_CreatesSchedule()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var schedules = new Mock<ITripScheduleRepository>();
        var assignments = new Mock<IRouteAssignmentRepository>();
        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        TripSchedule? created = null;

        assignments.Setup(x => x.GetByIdAsync(assignment.Id)).ReturnsAsync(assignment);
        schedules.Setup(x => x.ExistsOverlapAsync(
                assignment.Id,
                TripDirection.ToSchool,
                It.IsAny<DateOnly>(),
                null,
                true,
                true,
                true,
                true,
                true,
                false,
                false,
                null))
            .ReturnsAsync(false);
        schedules.Setup(x => x.AddAsync(It.IsAny<TripSchedule>()))
            .Callback<TripSchedule>(schedule => created = schedule)
            .Returns(Task.CompletedTask);

        var handler = new CreateTripScheduleHandler(schedules.Object, assignments.Object, audit.Object);

        var id = await handler.Handle(CreateCommand(assignment.Id), CancellationToken.None);

        id.Should().NotBeEmpty();
        created.Should().NotBeNull();
        created!.RouteAssignmentId.Should().Be(assignment.Id);
        created.Direction.Should().Be(TripDirection.ToSchool);
        created.IsActive.Should().BeTrue();
        schedules.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTripSchedule_Deactivates_WhenScheduleHasTrips()
    {
        var schedule = new TripSchedule(
            Guid.NewGuid(),
            TripDirection.FromSchool,
            new TimeOnly(13, 30),
            null,
            DateOnly.FromDateTime(DateTime.UtcNow));

        var schedules = new Mock<ITripScheduleRepository>();
        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        schedules.Setup(x => x.GetByIdAsync(schedule.Id)).ReturnsAsync(schedule);
        schedules.Setup(x => x.HasTripsAsync(schedule.Id)).ReturnsAsync(true);

        var handler = new DeleteTripScheduleHandler(schedules.Object, audit.Object);

        await handler.Handle(new DeleteTripScheduleCommand { Id = schedule.Id }, CancellationToken.None);

        schedule.IsActive.Should().BeFalse();
        schedules.Verify(x => x.Delete(It.IsAny<TripSchedule>()), Times.Never);
        schedules.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MaterializeTripSchedule_Fails_WhenScheduleDoesNotExist()
    {
        var schedules = new Mock<ITripScheduleRepository>();
        var trips = new Mock<ITripRepository>();
        var nonSchoolDays = new Mock<INonSchoolDayRepository>();
        var audit = new Mock<IAuditService>();
        var handler = new MaterializeTripScheduleHandler(
            schedules.Object,
            trips.Object,
            nonSchoolDays.Object,
            audit.Object,
            new ImmediateUnitOfWork());

        var act = () => handler.Handle(new MaterializeTripScheduleCommand
        {
            TripScheduleId = Guid.NewGuid(),
            OperationDate = new DateOnly(2026, 6, 8)
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task MaterializeTripSchedule_Fails_WhenScheduleIsInactive()
    {
        var schedule = CreateScheduleWithAssignment();
        schedule.Deactivate();
        var handler = CreateMaterializeHandler(schedule);

        var act = () => handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 8)), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task MaterializeTripSchedule_Fails_WhenDateIsOutsideValidity()
    {
        var schedule = CreateScheduleWithAssignment(validFrom: new DateOnly(2026, 6, 8), validTo: new DateOnly(2026, 6, 30));
        var handler = CreateMaterializeHandler(schedule);

        var act = () => handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 7, 1)), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task MaterializeTripSchedule_Fails_WhenDateIsNotActiveDay()
    {
        var schedule = CreateScheduleWithAssignment(mondayOnly: true);
        var handler = CreateMaterializeHandler(schedule);

        var act = () => handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 9)), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task MaterializeTripSchedule_Fails_WhenTripAlreadyExists()
    {
        var schedule = CreateScheduleWithAssignment();
        var handler = CreateMaterializeHandler(schedule, exists: true);

        var act = () => handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 8)), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task MaterializeTripSchedule_CreatesScheduledTrip()
    {
        var schedule = CreateScheduleWithAssignment();
        Trip? created = null;
        var handler = CreateMaterializeHandler(schedule, onAdd: trip => created = trip);

        var result = await handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 8)), CancellationToken.None);

        created.Should().NotBeNull();
        created!.Status.Should().Be(TripStatus.Scheduled);
        created.TripScheduleId.Should().Be(schedule.Id);
        created.Direction.Should().Be(schedule.Direction);
        created.OperationDate.Should().Be(new DateOnly(2026, 6, 8));
        created.ScheduledDepartureTime.Should().Be(new DateTime(2026, 6, 8, 6, 30, 0));
        created.ScheduledArrivalTime.Should().Be(new DateTime(2026, 6, 8, 7, 30, 0));
        result.Status.Should().Be(TripStatus.Scheduled);
        result.TripScheduleId.Should().Be(schedule.Id);
    }

    [Fact]
    public async Task MaterializeTripSchedule_OnSaturday_CreatesNotOperatingTrip()
    {
        var schedule = CreateScheduleWithAssignment();
        Trip? created = null;
        var handler = CreateMaterializeHandler(schedule, onAdd: trip => created = trip);

        var result = await handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 13)), CancellationToken.None);

        created.Should().NotBeNull();
        created!.Status.Should().Be(TripStatus.NotOperating);
        created.NonOperationReason.Should().Be("Weekend");
        result.Status.Should().Be(TripStatus.NotOperating);
    }

    [Fact]
    public async Task MaterializeTripSchedule_OnSunday_CreatesNotOperatingTrip()
    {
        var schedule = CreateScheduleWithAssignment();
        Trip? created = null;
        var handler = CreateMaterializeHandler(schedule, onAdd: trip => created = trip);

        await handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 14)), CancellationToken.None);

        created.Should().NotBeNull();
        created!.Status.Should().Be(TripStatus.NotOperating);
        created.NonOperationReason.Should().Be("Weekend");
    }

    [Fact]
    public async Task MaterializeTripSchedule_OnGlobalNonSchoolDay_CreatesNotOperatingTrip()
    {
        var schedule = CreateScheduleWithAssignment();
        var nonSchoolDay = new NonSchoolDay(new DateOnly(2026, 6, 10), NonSchoolDayReason.Holiday, "Feriado nacional");
        Trip? created = null;
        var handler = CreateMaterializeHandler(schedule, nonSchoolDay: nonSchoolDay, onAdd: trip => created = trip);

        await handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 10)), CancellationToken.None);

        created.Should().NotBeNull();
        created!.Status.Should().Be(TripStatus.NotOperating);
        created.NonOperationReason.Should().Be(nameof(NonSchoolDayReason.Holiday));
        created.NonOperationNotes.Should().Be("Feriado nacional");
    }

    [Fact]
    public async Task MaterializeTripSchedule_OnSchoolNonSchoolDay_CreatesNotOperatingTrip()
    {
        var schoolId = Guid.NewGuid();
        var schedule = CreateScheduleWithAssignment(schoolId: schoolId);
        var nonSchoolDay = new NonSchoolDay(new DateOnly(2026, 6, 11), NonSchoolDayReason.SchoolSuspension, "Suspensión del centro", schoolId);
        Trip? created = null;
        var handler = CreateMaterializeHandler(schedule, nonSchoolDay: nonSchoolDay, onAdd: trip => created = trip);

        await handler.Handle(MaterializeCommand(schedule.Id, new DateOnly(2026, 6, 11)), CancellationToken.None);

        created.Should().NotBeNull();
        created!.Status.Should().Be(TripStatus.NotOperating);
        created.NonOperationReason.Should().Be(nameof(NonSchoolDayReason.SchoolSuspension));
        created.NonOperationNotes.Should().Be("Suspensión del centro");
    }

    private static CreateTripScheduleCommand CreateCommand(Guid assignmentId)
    {
        return new CreateTripScheduleCommand
        {
            RouteAssignmentId = assignmentId,
            Direction = TripDirection.ToSchool,
            DepartureTime = new TimeOnly(6, 30),
            ArrivalTime = new TimeOnly(7, 30),
            ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow)
        };
    }

    private static MaterializeTripScheduleCommand MaterializeCommand(Guid scheduleId, DateOnly operationDate)
    {
        return new MaterializeTripScheduleCommand
        {
            TripScheduleId = scheduleId,
            OperationDate = operationDate
        };
    }

    private static TripSchedule CreateScheduleWithAssignment(
        DateOnly? validFrom = null,
        DateOnly? validTo = null,
        bool mondayOnly = false,
        Guid? schoolId = null)
    {
        var assignment = DomainTestFactory.AssignmentWithDetails(schoolId: schoolId);
        var schedule = new TripSchedule(
            assignment.Id,
            TripDirection.ToSchool,
            new TimeOnly(6, 30),
            new TimeOnly(7, 30),
            validFrom ?? new DateOnly(2026, 6, 1),
            validTo,
            true,
            !mondayOnly,
            !mondayOnly,
            !mondayOnly,
            !mondayOnly,
            false,
            false);

        ReflectionHelper.SetProperty(schedule, nameof(TripSchedule.RouteAssignment), assignment);
        return schedule;
    }

    private static MaterializeTripScheduleHandler CreateMaterializeHandler(
        TripSchedule schedule,
        bool exists = false,
        NonSchoolDay? nonSchoolDay = null,
        Action<Trip>? onAdd = null)
    {
        var schedules = new Mock<ITripScheduleRepository>();
        var trips = new Mock<ITripRepository>();
        var nonSchoolDays = new Mock<INonSchoolDayRepository>();
        var audit = new Mock<IAuditService>();

        schedules.Setup(x => x.GetByIdWithAssignmentDetailsAsync(schedule.Id)).ReturnsAsync(schedule);
        trips.Setup(x => x.ExistsByScheduleAndDateAsync(schedule.Id, It.IsAny<DateOnly>())).ReturnsAsync(exists);
        nonSchoolDays.Setup(x => x.GetActiveForDateAsync(It.IsAny<DateOnly>(), It.IsAny<Guid?>())).ReturnsAsync(nonSchoolDay);
        trips.Setup(x => x.AddAsync(It.IsAny<Trip>()))
            .Callback<Trip>(trip => onAdd?.Invoke(trip))
            .Returns(Task.CompletedTask);
        audit.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        return new MaterializeTripScheduleHandler(
            schedules.Object,
            trips.Object,
            nonSchoolDays.Object,
            audit.Object,
            new ImmediateUnitOfWork());
    }
}
