using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class NonSchoolDay : BaseEntity
    {
        public DateOnly Date { get; private set; }
        public Guid? SchoolId { get; private set; }
        public NonSchoolDayReason ReasonType { get; private set; }
        public string Reason { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public School? School { get; private set; }

        private NonSchoolDay() { }

        public NonSchoolDay(DateOnly date, NonSchoolDayReason reasonType, string reason, Guid? schoolId = null)
        {
            Update(date, reasonType, reason, schoolId);
            IsActive = true;
        }

        public void Update(DateOnly date, NonSchoolDayReason reasonType, string reason, Guid? schoolId = null)
        {
            if (date == default)
                throw new DomainException("La fecha es obligatoria");

            if (!Enum.IsDefined(typeof(NonSchoolDayReason), reasonType))
                throw new DomainException("El tipo de razón no es válido");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("La razón es obligatoria");

            Date = date;
            SchoolId = schoolId;
            ReasonType = reasonType;
            Reason = reason.Trim();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
