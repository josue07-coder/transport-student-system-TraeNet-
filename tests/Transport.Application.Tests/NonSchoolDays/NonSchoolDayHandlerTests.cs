using FluentAssertions;
using Moq;
using Transport.Application.Features.NonSchoolDays.Commands.CreateNonSchoolDay;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.NonSchoolDays;

public class NonSchoolDayHandlerTests
{
    [Fact]
    public async Task CreateNonSchoolDay_Fails_WhenDuplicateActiveExists()
    {
        var repository = new Mock<INonSchoolDayRepository>();
        var schools = new Mock<ISchoolRepository>();
        var audit = new Mock<IAuditService>();
        var command = CreateCommand();

        repository.Setup(x => x.ExistsActiveAsync(command.Date, command.SchoolId, null)).ReturnsAsync(true);
        var handler = new CreateNonSchoolDayHandler(repository.Object, schools.Object, audit.Object);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task CreateNonSchoolDay_CreatesCorrectly()
    {
        var repository = new Mock<INonSchoolDayRepository>();
        var schools = new Mock<ISchoolRepository>();
        var audit = new Mock<IAuditService>();
        var command = CreateCommand();
        NonSchoolDay? created = null;

        repository.Setup(x => x.ExistsActiveAsync(command.Date, command.SchoolId, null)).ReturnsAsync(false);
        repository.Setup(x => x.AddAsync(It.IsAny<NonSchoolDay>()))
            .Callback<NonSchoolDay>(day => created = day)
            .Returns(Task.CompletedTask);
        audit.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateNonSchoolDayHandler(repository.Object, schools.Object, audit.Object);

        var id = await handler.Handle(command, CancellationToken.None);

        id.Should().NotBeEmpty();
        created.Should().NotBeNull();
        created!.Date.Should().Be(command.Date);
        created.ReasonType.Should().Be(command.ReasonType);
        created.Reason.Should().Be(command.Reason);
        repository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    private static CreateNonSchoolDayCommand CreateCommand()
    {
        return new CreateNonSchoolDayCommand
        {
            Date = new DateOnly(2026, 6, 10),
            ReasonType = NonSchoolDayReason.Holiday,
            Reason = "Feriado nacional"
        };
    }
}
