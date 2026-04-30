using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Domain.Exceptions;

namespace Transport.Domain.ValueObjects
{
    public class StudentCode : ValueObject
    {
        public string Value { get; }

        private StudentCode(string value)
        {
            Value = value;
        }

        public static StudentCode Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Code is required");

            return new StudentCode(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
