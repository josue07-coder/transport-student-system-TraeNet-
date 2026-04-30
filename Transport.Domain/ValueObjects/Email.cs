using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Domain.Exceptions;

namespace Transport.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Email is required");

            if (!value.Contains("@"))
                throw new DomainException("Invalid email format");

            return new Email(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }
    }
}
