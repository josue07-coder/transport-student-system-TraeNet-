using FluentAssertions;
using Moq;
using Transport.Application.Features.TripStudentAttendances.Commands;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentAbsent;
using Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentBoarded;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Trips;

public class TripStudentAttendanceCommandHandlerTests
{
    [Fact]
    public async Task MarkBoarded_Fails_WhenTripIsNotInProgress()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var trip = new Trip(assignment.Id);
        ReflectionHelper.SetProperty(trip, nameof(Trip.RouteAssignment), assignment);
        trip.Start();
        trip.End();

        var context = CreateContext(trip, null);
        var handler = CreateHandler(context);

        var act = () => handler.Handle(new MarkStudentBoardedCommand
        {
            TripId = trip.Id,
            StudentId = Guid.NewGuid()
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Solo se puede actualizar la asistencia en un viaje en progreso");

        context.AttendanceRepository.Verify(
            x => x.GetByTripAndStudentAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task MarkAbsent_Fails_WhenStudentAlreadyBoarded()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var trip = new Trip(assignment.Id);
        ReflectionHelper.SetProperty(trip, nameof(Trip.RouteAssignment), assignment);
        trip.Start();

        var studentId = Guid.NewGuid();
        var attendance = new TripStudentAttendance(
            trip.Id,
            studentId,
            "Ana Santos",
            "STU-001",
            Guid.NewGuid(),
            "Maria Santos");
        attendance.MarkBoarded(DateTime.UtcNow);

        var context = CreateContext(trip, attendance);
        var handler = CreateHandler(context);

        var act = () => handler.Handle(new MarkStudentAbsentCommand
        {
            TripId = trip.Id,
            StudentId = studentId,
            Notes = "No asistió"
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("No se puede marcar ausente un estudiante que ya abordó");
    }

    private static TripStudentAttendanceCommandHandler CreateHandler(TestContext context)
    {
        return new TripStudentAttendanceCommandHandler(
            context.TripRepository.Object,
            context.AttendanceRepository.Object,
            context.AuditService.Object,
            context.OperationAuthorizationService.Object,
            context.CurrentUserService.Object,
            context.UserRepository.Object,
            context.NotificationService.Object);
    }

    private static TestContext CreateContext(Trip trip, TripStudentAttendance? attendance)
    {
        var context = new TestContext();
        context.TripRepository.Setup(x => x.GetByIdWithAssignmentDetailsAsync(trip.Id)).ReturnsAsync(trip);
        context.AttendanceRepository.Setup(x => x.GetByTripAndStudentAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(attendance);
        context.AttendanceRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        context.OperationAuthorizationService.Setup(x => x.EnsureCanManageTripAttendanceAsync(trip))
            .Returns(Task.CompletedTask);
        context.CurrentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        context.AuditService.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        return context;
    }

    private sealed class TestContext
    {
        public Mock<ITripRepository> TripRepository { get; } = new();
        public Mock<ITripStudentAttendanceRepository> AttendanceRepository { get; } = new();
        public Mock<IAuditService> AuditService { get; } = new();
        public Mock<ITripOperationAuthorizationService> OperationAuthorizationService { get; } = new();
        public Mock<ICurrentUserService> CurrentUserService { get; } = new();
        public Mock<IUserRepository> UserRepository { get; } = new();
        public Mock<INotificationService> NotificationService { get; } = new();
    }
}
