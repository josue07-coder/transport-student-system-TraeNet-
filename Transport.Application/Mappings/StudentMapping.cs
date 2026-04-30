using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Mappings
{
    public static class StudentMapping
    {
        public static StudentDto ToDto(this Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                FullName = $"{student.FirstName} {student.LastName}",
                Code = student.StudentCode.Value,
                SchoolId = student.SchoolId
            };
        }
    }
}
