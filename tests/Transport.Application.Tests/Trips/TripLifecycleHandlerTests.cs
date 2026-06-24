using FluentAssertions;
using Moq;
using Transport.Application.Features.Trips.Commands.CancelTrip;
using Transport.Application.Features.Trips.Commands.EndTrip;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Trips;

public class TripLifecycleHandlerTests
{
    [Fact]
    public async Task CancelTrip_Fails_WhenReasonIsMissing()
    {
        var trip = new Trip(Guid.NewGuid());
        var handler = CreateCancelHandler(trip);

        var act = () => handler.Handle(new CancelTripCommand { Id = trip.Id, Reason = "" }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task CancelTrip_Fails_WhenTripIsInProgress()
    {
        var trip = new Trip(Guid.NewGuid());
        trip.Start();
        var handler = CreateCancelHandler(trip);

        var act = () => handler.Handle(new CancelTripCommand { Id = trip.Id, Reason = "Clases suspendidas" }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task EndTrip_Fails_WhenTripIsScheduled()
    {
        var trip = new Trip(Guid.NewGuid());
        var repository = new Mock<ITripRepository>();
        repository.Setup(x => x.GetByIdAsync(trip.Id)).ReturnsAsync(trip);
        var handler = new EndTripHandler(
            repository.Object,
            Mock.Of<IAuditService>(),
            Mock.Of<INotificationService>(),
            Mock.Of<IUserRepository>(),
            Mock.Of<ITripOperationAuthorizationService>());

        var act = () => handler.Handle(new EndTripCommand { Id = trip.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    private static CancelTripHandler CreateCancelHandler(Trip trip)
    {
        var repository = new Mock<ITripRepository>();
        repository.Setup(x => x.GetByIdAsync(trip.Id)).ReturnsAsync(trip);

        return new CancelTripHandler(
            repository.Object,
            Mock.Of<IAuditService>(),
            Mock.Of<INotificationService>(),
            Mock.Of<IUserRepository>());
    }
}
