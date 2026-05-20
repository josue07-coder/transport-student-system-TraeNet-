using MediatR;
using Transport.Application.Features.Me.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Me.Commands.UpdateMeProfile
{
    public class UpdateMeProfileHandler : IRequestHandler<UpdateMeProfileCommand, MeProfileDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public UpdateMeProfileHandler(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<MeProfileDto> Handle(UpdateMeProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            var user = await _userRepository.GetByIdWithRoleAsync(userId)
                ?? throw new DomainException("Usuario no encontrado");

            user.UpdateProfile(request.Name, request.ProfileImageUrl);
            user.SetEmail(request.Email);

            await _userRepository.SaveChangesAsync();

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
