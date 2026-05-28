using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Tests.Testing;

internal static class DomainTestFactory
{
    public static Route ActiveRoute(Guid? schoolId = null, bool withStops = true)
    {
        var start = DateTime.UtcNow.Date.AddHours(7);
        var route = new Route("Ruta Norte", schoolId ?? Guid.NewGuid(), new TimeRange(start, start.AddHours(1)));

        if (withStops)
            route.AddStop(new RouteStop(route.Id, Guid.NewGuid(), 1));

        ReflectionHelper.SetProperty(route, nameof(Route.Status), RouteStatus.Active);
        return route;
    }

    public static Driver ActiveDriver()
    {
        return new Driver(
            "Carlos",
            "Perez",
            DocumentType.Cedula,
            "00100000001",
            new LicenseNumber("LIC-001"),
            PhoneNumber.Create("8095550001"),
            Address.Create("Calle 1", "Santo Domingo"),
            Email.Create("driver@test.local"));
    }

    public static Vehicle ActiveVehicle(int capacity = 20)
    {
        return new Vehicle("A123456", capacity);
    }

    public static Student ActiveStudent(Guid schoolId, Guid? guardianId = null)
    {
        return new Student(
            "Ana",
            "Santos",
            StudentCode.Create($"STU-{Guid.NewGuid():N}"[..12]),
            schoolId,
            Guid.NewGuid(),
            guardianId ?? Guid.NewGuid());
    }

    public static RouteAssignment AssignmentWithDetails(int capacity = 2, Guid? schoolId = null)
    {
        var route = ActiveRoute(schoolId);
        var assignment = new RouteAssignment(route.Id, Guid.NewGuid(), Guid.NewGuid(), capacity);
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.Route), route);
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.Driver), ActiveDriver());
        ReflectionHelper.SetProperty(assignment, nameof(RouteAssignment.Vehicle), ActiveVehicle(capacity));
        return assignment;
    }
}
