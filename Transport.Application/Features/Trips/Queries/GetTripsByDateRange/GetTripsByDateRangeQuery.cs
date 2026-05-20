using MediatR;
using Transport.Application.Features.Trips.DTOs;

namespace Transport.Application.Features.Trips.Queries.GetTripsByDateRange
{
    public record GetTripsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<TripResponseDto>>;
}
