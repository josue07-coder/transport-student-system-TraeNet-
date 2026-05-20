using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.TransportAssistants.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.TransportAssistants.Queries.GetAllTransportAssistants
{
    public class GetAllTransportAssistantsHandler : IRequestHandler<GetAllTransportAssistantsQuery, PaginatedResponse<TransportAssistantResponseDto>>
    {
        private readonly ITransportAssistantRepository _repository;

        public GetAllTransportAssistantsHandler(ITransportAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<TransportAssistantResponseDto>> Handle(GetAllTransportAssistantsQuery request, CancellationToken cancellationToken)
        {
            var assistants = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = assistants.Items.Select(TransportAssistantMappings.ToResponseDto).ToList();

            return new PaginatedResponse<TransportAssistantResponseDto>(items, assistants.TotalCount, assistants.PageNumber, assistants.PageSize);
        }
    }
}
