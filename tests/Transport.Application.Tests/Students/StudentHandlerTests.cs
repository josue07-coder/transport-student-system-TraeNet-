using FluentAssertions;
using Moq;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Features.Students.Commands.DeleteStudent;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Students;

public class StudentHandlerTests
{
    [Fact]
    public async Task CreateStudent_Fails_WhenGradeDoesNotBelongToSchool()
    {
        var studentRepo = new Mock<IStudentRepository>();
        var guardians = new Mock<IGuardianRepository>();
        var schools = new Mock<ISchoolRepository>();
        var grades = new Mock<IGradeRepository>();
        var audit = new Mock<IAuditService>();

        var schoolId = Guid.NewGuid();
        var gradeId = Guid.NewGuid();
        guardians.Setup(x => x.IsActiveAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        schools.Setup(x => x.IsActiveAsync(schoolId)).ReturnsAsync(true);
        grades.Setup(x => x.BelongsToSchoolAsync(gradeId, schoolId)).ReturnsAsync(false);

        var handler = new CreateStudentHandler(studentRepo.Object, guardians.Object, schools.Object, grades.Object, audit.Object);

        var act = () => handler.Handle(new CreateStudentCommand
        {
            FirstName = "Ana",
            LastName = "Santos",
            GuardianId = Guid.NewGuid(),
            SchoolId = schoolId,
            GradeId = gradeId
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task DeleteStudent_Fails_WhenStudentHasTripInProgress()
    {
        var student = Transport.Application.Tests.Testing.DomainTestFactory.ActiveStudent(Guid.NewGuid());
        var repo = new Mock<IStudentRepository>();
        var audit = new Mock<IAuditService>();
        repo.Setup(x => x.GetByIdAsync(student.Id)).ReturnsAsync(student);
        repo.Setup(x => x.HasInProgressTripAsync(student.Id)).ReturnsAsync(true);

        var handler = new DeleteStudentHandler(repo.Object, audit.Object);

        var act = () => handler.Handle(new DeleteStudentCommand { Id = student.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
