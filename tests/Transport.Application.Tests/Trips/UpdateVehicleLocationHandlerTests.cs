using FluentAssertions;
using Moq;
using Transport.Application.Features.Tracking.Commands.UpdateVehicleLocation;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Trips;

public class UpdateVehicleLocationHandlerTests
{
    [Fact]
    public async Task UpdateVehicleLocation_Fails_WhenTripIsCompleted()
    {
        var assignment = DomainTestFactory.AssignmentWithDetails();
        var trip = new Trip(assignment.Id);
        ReflectionHelper.SetProperty(trip, nameof(Trip.RouteAssignment), assignment);
        trip.Start();
        trip.End();

        var trips = new Mock<ITripRepository>();
        trips.Setup(x => x.GetByIdWithAssignmentDetailsAsync(trip.Id)).ReturnsAsync(trip);

        var locations = new Mock<IVehicleLocationRepository>();
        var handler = new UpdateVehicleLocationHandler(
            trips.Object,
            locations.Object,
            Mock.Of<ICurrentUserService>(),
            Mock.Of<INotificationService>(),
            Mock.Of<ISystemSettingService>(),
            Mock.Of<ITripOperationAuthorizationService>());

        var act = () => handler.Handle(new UpdateVehicleLocationCommand
        {
            TripId = trip.Id,
            Latitude = 18.2081m,
            Longitude = -71.1002m
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Solo se puede registrar*viaje en progreso");

        locations.Verify(x => x.AddAsync(It.IsAny<VehicleLocation>()), Times.Never);
    }
}
