using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public ForgotPasswordHandler(IUserRepository userRepository, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.DocumentNumber)
                ?? throw new DomainException("Usuario no encontrado");

            var userWithProfiles = await _userRepository.GetByIdWithLinkedProfilesAsync(user.Id)
                ?? throw new DomainException("Usuario no encontrado");

            if (!MatchesLinkedContact(userWithProfiles, request.PhoneOrEmail))
                throw new DomainException("El contacto indicado no coincide con el usuario");

            userWithProfiles.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }

        private static bool MatchesLinkedContact(Domain.Entities.User user, string phoneOrEmail)
        {
            var value = Normalize(phoneOrEmail);

            if (user.Guardian is not null && Normalize(user.Guardian.Phone) == value)
                return true;

            if (user.Driver is not null)
            {
                if (Normalize(user.Driver.Phone.Value) == value)
                    return true;

                if (user.Driver.Email is not null && Normalize(user.Driver.Email.Value) == value)
                    return true;
            }

            if (user.TransportAssistant is not null)
            {
                if (Normalize(user.TransportAssistant.Phone.Value) == value)
                    return true;

                if (user.TransportAssistant.Email is not null && Normalize(user.TransportAssistant.Email.Value) == value)
                    return true;
            }

            return false;
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToLowerInvariant();
        }
    }
}
