using MediatR;
using Transport.Application.Features.Me.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Me.Queries.GetMe
{
    public class GetMeHandler : IRequestHandler<GetMeQuery, MeProfileDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public GetMeHandler(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<MeProfileDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            var user = await _userRepository.GetByIdWithRoleAsync(userId)
                ?? throw new DomainException("Usuario no encontrado");

            return new MeProfileDto
            {
                UserId = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.Name,
                ProfileImageUrl = user.ProfileImageUrl,
                GuardianId = user.GuardianId,
                DriverId = user.DriverId,
                TransportAssistantId = user.TransportAssistantId
            };
        }
    }
}
