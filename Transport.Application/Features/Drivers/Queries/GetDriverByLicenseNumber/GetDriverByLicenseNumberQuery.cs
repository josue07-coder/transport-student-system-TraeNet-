using MediatR;
using Transport.Application.Features.Drivers.DTOs;

namespace Transport.Application.Features.Drivers.Queries.GetDriverByLicenseNumber
{
    public record GetDriverByLicenseNumberQuery(string LicenseNumber) : IRequest<DriverDetailDto>;
}
