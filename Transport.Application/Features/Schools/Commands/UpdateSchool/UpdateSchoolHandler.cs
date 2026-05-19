using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Schools.Commands.UpdateSchool
{
    public class UpdateSchoolHandler : IRequestHandler<UpdateSchoolCommand, Unit>
    {
        private readonly ISchoolRepository _repository;

        public UpdateSchoolHandler(ISchoolRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
        {
            var school = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("School not found");

            school.SetName(request.Name);
            school.SetDirectorName(request.DirectorName);
            school.UpdateContact(Email.Create(request.Email), PhoneNumber.Create(request.Phone));
            school.UpdateAddress(Address.Create(request.Street, request.City));
            school.ChangeSector(request.SectorId);
            school.UpdateProfile(request.Description, request.ProfileImageUrl);

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
