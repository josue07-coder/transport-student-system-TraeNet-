using Transport.Application.Features.Drivers.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.Drivers.Queries
{
    internal static class DriverMappings
    {
        public static DriverResponseDto ToResponseDto(Driver driver)
        {
            return new DriverResponseDto
            {
                Id = driver.Id,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                FullName = $"{driver.FirstName} {driver.LastName}",
                DocumentType = driver.DocumentType,
                DocumentNumber = driver.DocumentNumber,
                LicenseNumber = driver.LicenseNumber.Value,
                Phone = driver.Phone.Value,
                Street = driver.Address.Street,
                City = driver.Address.City,
                Email = driver.Email?.Value,
                PhotoUrl = driver.PhotoUrl,
                IsActive = driver.IsActive
            };
        }

        public static DriverDetailDto ToDetailDto(Driver driver)
        {
            return new DriverDetailDto
            {
                Id = driver.Id,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                FullName = $"{driver.FirstName} {driver.LastName}",
                DocumentType = driver.DocumentType,
                DocumentNumber = driver.DocumentNumber,
                LicenseNumber = driver.LicenseNumber.Value,
                Phone = driver.Phone.Value,
                Street = driver.Address.Street,
                City = driver.Address.City,
                Email = driver.Email?.Value,
                PhotoUrl = driver.PhotoUrl,
                IsActive = driver.IsActive
            };
        }
    }
}
