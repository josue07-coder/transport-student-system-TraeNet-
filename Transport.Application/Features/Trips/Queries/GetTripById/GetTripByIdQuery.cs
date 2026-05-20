using MediatR;
using Transport.Application.Features.Trips.DTOs;

namespace Transport.Application.Features.Trips.Queries.GetTripById
{
    public record GetTripByIdQuery(Guid Id) : IRequest<TripDetailDto>;
}
