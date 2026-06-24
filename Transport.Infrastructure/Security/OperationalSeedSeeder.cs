using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.ValueObjects;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Security
{
    public static class OperationalSeedSeeder
    {
        private const string SupervisorUsername = "supervisor";
        private const string DriverUsername = "driver01";
        private const string AssistantUsername = "assistant01";
        private const string GuardianUsername = "guardian01";

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

            var roles = await context.Roles.ToDictionaryAsync(role => role.Name);

            var district = await GetOrCreateDistrictAsync(context);
            var sector = await GetOrCreateSectorAsync(context, district.Id);
            var school = await GetOrCreateSchoolAsync(context, sector.Id);
            var grade = await GetOrCreateGradeAsync(context, school.Id);
            var guardian = await GetOrCreateGuardianAsync(context, sector.Id);
            var student = await GetOrCreateStudentAsync(context, school.Id, grade.Id, guardian.Id);
            var driver = await GetOrCreateDriverAsync(context);
            var assistant = await GetOrCreateAssistantAsync(context);
            var vehicle = await GetOrCreateVehicleAsync(context);
            var stop = await GetOrCreateStopAsync(context, sector.Id);
            var route = await GetOrCreateRouteAsync(context, school.Id, stop.Id);
            var assignment = await GetOrCreateAssignmentAsync(context, route.Id, vehicle.Id, driver.Id, assistant.Id, student.Id);

            await EnsureUserAsync(context, passwordHasher, roles, "Supervisor", SupervisorUsername, "Supervisor Operativo", "supervisor@trae.local", "Supervisor123");
            await EnsureUserAsync(context, passwordHasher, roles, "Driver", DriverUsername, "Conductor Operativo", "driver01@trae.local", "Driver123", driverId: driver.Id);
            await EnsureUserAsync(context, passwordHasher, roles, "TransportAssistant", AssistantUsername, "Asistente Operativo", "assistant01@trae.local", "Assistant123", transportAssistantId: assistant.Id);
            await EnsureUserAsync(context, passwordHasher, roles, "Guardian", GuardianUsername, "Tutor Operativo", "guardian01@trae.local", "Guardian123", guardianId: guardian.Id);

            await EnsureTripsAsync(context, assignment.Id, student);

            await context.SaveChangesAsync();
        }

        private static async Task<SchoolDistrict> GetOrCreateDistrictAsync(AppDbContext context)
        {
            var district = await context.SchoolDistricts.FirstOrDefaultAsync(item => item.Code == "01-03-TRAE");
            if (district is not null)
                return district;

            district = new SchoolDistrict(
                "Distrito Educativo 01-03 TRAE",
                "01-03-TRAE",
                Address.Create("Avenida Duarte", "Barahona"),
                "Distrito semilla para pruebas operativas");

            await context.SchoolDistricts.AddAsync(district);
            await context.SaveChangesAsync();
            return district;
        }

        private static async Task<Sector> GetOrCreateSectorAsync(AppDbContext context, Guid districtId)
        {
            var sector = await context.Sectors.FirstOrDefaultAsync(item => item.Name == "Sector TRAE Operativo");
            if (sector is not null)
                return sector;

            sector = new Sector("Sector TRAE Operativo", "Barahona", "Barahona", districtId);
            await context.Sectors.AddAsync(sector);
            await context.SaveChangesAsync();
            return sector;
        }

        private static async Task<School> GetOrCreateSchoolAsync(AppDbContext context, Guid sectorId)
        {
            var school = await context.Schools.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Name == "Centro Educativo TRAE");
            if (school is not null)
                return school;

            school = new School(
                "Centro Educativo TRAE",
                "Director TRAE",
                Email.Create("centro.trae@test.local"),
                PhoneNumber.Create("8095551000"),
                Address.Create("Calle Escolar 1", "Barahona"),
                sectorId,
                "Centro semilla para pruebas funcionales");

            await context.Schools.AddAsync(school);
            await context.SaveChangesAsync();
            return school;
        }

        private static async Task<Grade> GetOrCreateGradeAsync(AppDbContext context, Guid schoolId)
        {
            var grade = await context.Grades.FirstOrDefaultAsync(item => item.Name == "Sexto A TRAE" && item.SchoolId == schoolId);
            if (grade is not null)
                return grade;

            grade = new Grade("Sexto A TRAE", schoolId);
            await context.Grades.AddAsync(grade);
            await context.SaveChangesAsync();
            return grade;
        }

        private static async Task<Guardian> GetOrCreateGuardianAsync(AppDbContext context, Guid sectorId)
        {
            var guardian = await context.Guardians.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.DocumentNumber == "GUA-TRAE-001");
            if (guardian is not null)
                return guardian;

            guardian = new Guardian(
                DocumentType.Cedula,
                "GUA-TRAE-001",
                "Tutor",
                "Operativo",
                "8095552000",
                Address.Create("Calle Tutor 1", "Barahona"),
                Gender.Female,
                sectorId);

            await context.Guardians.AddAsync(guardian);
            await context.SaveChangesAsync();
            return guardian;
        }

        private static async Task<Student> GetOrCreateStudentAsync(AppDbContext context, Guid schoolId, Guid gradeId, Guid guardianId)
        {
            var student = await context.Students.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.StudentCode.Value == "TRAE-STU-001");
            if (student is not null)
                return student;

            student = new Student(
                "Estudiante",
                "Operativo",
                StudentCode.Create("TRAE-STU-001"),
                schoolId,
                gradeId,
                guardianId);

            await context.Students.AddAsync(student);
            await context.SaveChangesAsync();
            return student;
        }

        private static async Task<Driver> GetOrCreateDriverAsync(AppDbContext context)
        {
            var driver = await context.Drivers.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.DocumentNumber == "DRV-TRAE-001");
            if (driver is not null)
                return driver;

            driver = new Driver(
                "Conductor",
                "Operativo",
                DocumentType.Cedula,
                "DRV-TRAE-001",
                new LicenseNumber("LIC-TRAE-001"),
                PhoneNumber.Create("8095553000"),
                Address.Create("Calle Conductor 1", "Barahona"),
                Email.Create("driver01@trae.local"));

            await context.Drivers.AddAsync(driver);
            await context.SaveChangesAsync();
            return driver;
        }

        private static async Task<TransportAssistant> GetOrCreateAssistantAsync(AppDbContext context)
        {
            var assistant = await context.TransportAssistants.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.DocumentNumber == "AST-TRAE-001");
            if (assistant is not null)
                return assistant;

            assistant = new TransportAssistant(
                DocumentType.Cedula,
                "AST-TRAE-001",
                "Asistente",
                "Operativo",
                PhoneNumber.Create("8095554000"),
                Address.Create("Calle Asistente 1", "Barahona"),
                Email.Create("assistant01@trae.local"));

            await context.TransportAssistants.AddAsync(assistant);
            await context.SaveChangesAsync();
            return assistant;
        }

        private static async Task<Vehicle> GetOrCreateVehicleAsync(AppDbContext context)
        {
            var vehicle = await context.Vehicles.FirstOrDefaultAsync(item => item.PlateNumber == "TRAE-001");
            if (vehicle is not null)
                return vehicle;

            vehicle = new Vehicle("TRAE-001", 20);
            await context.Vehicles.AddAsync(vehicle);
            await context.SaveChangesAsync();
            return vehicle;
        }

        private static async Task<Stop> GetOrCreateStopAsync(AppDbContext context, Guid sectorId)
        {
            var stop = await context.Stops.FirstOrDefaultAsync(item => item.Name == "Parada TRAE Principal");
            if (stop is not null)
                return stop;

            stop = new Stop(
                "Parada TRAE Principal",
                Address.Create("Calle Parada 1", "Barahona"),
                Coordinates.Create(18.2081, -71.1002),
                sectorId);

            await context.Stops.AddAsync(stop);
            await context.SaveChangesAsync();
            return stop;
        }

        private static async Task<Route> GetOrCreateRouteAsync(AppDbContext context, Guid schoolId, Guid stopId)
        {
            var route = await context.Routes
                .Include(item => item.Stops)
                .FirstOrDefaultAsync(item => item.Name == "Ruta TRAE Operativa" && item.SchoolId == schoolId);

            if (route is null)
            {
                var today = DateTime.UtcNow.Date;
                route = new Route(
                    "Ruta TRAE Operativa",
                    schoolId,
                    new TimeRange(today.AddHours(6), today.AddHours(7)));

                route.AddStop(new RouteStop(route.Id, stopId, 1));
                route.Activate();
                await context.Routes.AddAsync(route);
                await context.SaveChangesAsync();
                return route;
            }

            if (!route.Stops.Any(item => item.StopId == stopId))
                route.AddStop(new RouteStop(route.Id, stopId, route.Stops.Count + 1));

            if (route.Status != RouteStatus.Active)
                route.Activate();

            await context.SaveChangesAsync();
            return route;
        }

        private static async Task<RouteAssignment> GetOrCreateAssignmentAsync(
            AppDbContext context,
            Guid routeId,
            Guid vehicleId,
            Guid driverId,
            Guid assistantId,
            Guid studentId)
        {
            var assignment = await context.RouteAssignments
                .Include(item => item.Students)
                .FirstOrDefaultAsync(item =>
                    item.RouteId == routeId &&
                    item.VehicleId == vehicleId &&
                    item.DriverId == driverId &&
                    item.TransportAssistantId == assistantId);

            if (assignment is null)
            {
                assignment = new RouteAssignment(routeId, vehicleId, driverId, 20, assistantId);
                assignment.AssignStudent(studentId);
                await context.RouteAssignments.AddAsync(assignment);
                await context.SaveChangesAsync();
                return assignment;
            }

            if (!assignment.Students.Any(item => item.StudentId == studentId))
                assignment.AssignStudent(studentId);

            await context.SaveChangesAsync();
            return assignment;
        }

        private static async Task EnsureUserAsync(
            AppDbContext context,
            IPasswordHasherService passwordHasher,
            Dictionary<string, Role> roles,
            string roleName,
            string username,
            string name,
            string email,
            string password,
            Guid? guardianId = null,
            Guid? driverId = null,
            Guid? transportAssistantId = null)
        {
            if (!roles.TryGetValue(roleName, out var role))
                return;

            var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Username == username);
            if (user is not null)
            {
                if (!user.IsActive)
                    user.Activate();

                return;
            }

            await context.Users.AddAsync(new User(
                username,
                name,
                email,
                passwordHasher.HashPassword(password),
                role.Id,
                guardianId,
                driverId,
                transportAssistantId));

            await context.SaveChangesAsync();
        }

        private static async Task EnsureTripsAsync(AppDbContext context, Guid assignmentId, Student student)
        {
            var hasScheduled = await context.Trips.AnyAsync(item => item.RouteAssignmentId == assignmentId && item.Status == TripStatus.Scheduled);
            if (!hasScheduled)
            {
                var scheduled = new Trip(assignmentId);
                await context.Trips.AddAsync(scheduled);
                await context.SaveChangesAsync();
            }

            var hasInProgress = await context.Trips.AnyAsync(item => item.RouteAssignmentId == assignmentId && item.Status == TripStatus.InProgress);
            if (!hasInProgress)
            {
                var inProgress = new Trip(assignmentId);
                inProgress.Start();
                await context.Trips.AddAsync(inProgress);
                await context.SaveChangesAsync();
                await EnsureAttendanceAsync(context, inProgress.Id, student);
            }

            var hasCompleted = await context.Trips.AnyAsync(item => item.RouteAssignmentId == assignmentId && item.Status == TripStatus.Completed);
            if (!hasCompleted)
            {
                var completed = new Trip(assignmentId);
                completed.Start(DateTime.UtcNow.AddMinutes(-30), 0, false, null);
                completed.End();
                await context.Trips.AddAsync(completed);
                await context.SaveChangesAsync();
                await EnsureAttendanceAsync(context, completed.Id, student);
            }
        }

        private static async Task EnsureAttendanceAsync(AppDbContext context, Guid tripId, Student student)
        {
            var exists = await context.TripStudentAttendances.AnyAsync(item => item.TripId == tripId && item.StudentId == student.Id);
            if (exists)
                return;

            await context.TripStudentAttendances.AddAsync(new TripStudentAttendance(
                tripId,
                student.Id,
                $"{student.FirstName} {student.LastName}",
                student.StudentCode.Value,
                student.GuardianId,
                "Tutor Operativo"));

            await context.SaveChangesAsync();
        }
    }
}
