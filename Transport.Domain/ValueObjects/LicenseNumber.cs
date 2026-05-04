using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Domain.Exceptions;

namespace Transport.Domain.ValueObjects
{
    public class LicenseNumber : ValueObject
    {
        public string Value { get; }

        private LicenseNumber() { }

        public LicenseNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("La licencia es obligatoria");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
