using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Transport.Domain.Common;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Persistence.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //  Académico
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Guardian> Guardians => Set<Guardian>();

        //  Transporte
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<RouteStop> RouteStops => Set<RouteStop>();
        public DbSet<RouteAssignment> RouteAssignments => Set<RouteAssignment>();
        public DbSet<StudentRouteAssignment> StudentRouteAssignments => Set<StudentRouteAssignment>();
        public DbSet<Trip> Trips => Set<Trip>();

        //  Operación
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<TransportAssistant> TransportAssistants => Set<TransportAssistant>();

        //  Geografía
        public DbSet<Stop> Stops => Set<Stop>();
        public DbSet<Sector> Sectors => Set<Sector>();
        public DbSet<SchoolDistrict> SchoolDistricts => Set<SchoolDistrict>();

        //  Institucional
        public DbSet<School> Schools => Set<School>();

        //  Seguridad
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<IncidentComment> IncidentComments => Set<IncidentComment>();
        public DbSet<VehicleLocation> VehicleLocations => Set<VehicleLocation>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            //  GLOBAL FILTER: IsActive
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IActivatable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(IActivatable.IsActive));
                    var condition = Expression.Equal(property, Expression.Constant(true));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}
