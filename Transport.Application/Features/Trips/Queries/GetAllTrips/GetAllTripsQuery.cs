using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Trips.DTOs;

namespace Transport.Application.Features.Trips.Queries.GetAllTrips
{
    public class GetAllTripsQuery : PaginationRequest, IRequest<PaginatedResponse<TripResponseDto>>
    {
    }
}
