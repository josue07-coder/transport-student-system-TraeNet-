using MediatR;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Guardians.Commands.CreateGuardian
{
    public record CreateGuardianCommand(
    DocumentType DocumentType,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Phone,
    string Street,
    string City,
    Gender Gender,
    Guid? SectorId
) : IRequest<Guid>;
}