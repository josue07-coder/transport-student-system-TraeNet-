using Transport.Application.Features.TransportAssistants.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.TransportAssistants.Queries
{
    internal static class TransportAssistantMappings
    {
        public static TransportAssistantResponseDto ToResponseDto(TransportAssistant assistant)
        {
            return new TransportAssistantResponseDto
            {
                Id = assistant.Id,
                FullName = $"{assistant.FirstName} {assistant.LastName}",
                DocumentNumber = assistant.DocumentNumber,
                Phone = assistant.Phone.Value,
                Email = assistant.Email?.Value,
                IsActive = assistant.IsActive
            };
        }

        public static TransportAssistantDetailDto ToDetailDto(TransportAssistant assistant)
        {
            return new TransportAssistantDetailDto
            {
                Id = assistant.Id,
                DocumentType = assistant.DocumentType,
                DocumentNumber = assistant.DocumentNumber,
                FirstName = assistant.FirstName,
                LastName = assistant.LastName,
                FullName = $"{assistant.FirstName} {assistant.LastName}",
                Phone = assistant.Phone.Value,
                Street = assistant.Address.Street,
                City = assistant.Address.City,
                Email = assistant.Email?.Value,
                PhotoUrl = assistant.PhotoUrl,
                IsActive = assistant.IsActive
            };
        }
    }
}
