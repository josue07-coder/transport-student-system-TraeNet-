using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Transport.Application;
using Transport.Application.Common.Behaviors;
using Transport.Application.Common.Security;
using Transport.Application.Interfaces;
using Transport.API.Services;
using Transport.Infrastructure.Integrations;
using Transport.Infrastructure.Persistence;
using Transport.Infrastructure.Persistence.Repositories;
using Transport.Infrastructure.Repositories;
using Transport.Infrastructure.Security;

namespace Transport.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //  Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ISchoolRepository, SchoolRepository>();
            services.AddScoped<IGradeRepository, GradeRepository>();
            services.AddScoped<IGuardianRepository, GuardianRepository>();
            services.AddScoped<ISectorRepository, SectorRepository>();
            services.AddScoped<IRouteAssignmentRepository, RouteAssignmentRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ITransportAssistantRepository, TransportAssistantRepository>();
            services.AddScoped<IStopRepository, StopRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<ITripRepository, TripRepository>();
            services.AddScoped<ITripScheduleRepository, TripScheduleRepository>();
            services.AddScoped<ITripStudentAttendanceRepository, TripStudentAttendanceRepository>();
            services.AddScoped<ITripRouteDeviationRepository, TripRouteDeviationRepository>();
            services.AddScoped<INonSchoolDayRepository, NonSchoolDayRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IIncidentRepository, IncidentRepository>();
            services.AddScoped<IVehicleLocationRepository, VehicleLocationRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
            services.AddScoped<IBackupRecordRepository, BackupRecordRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IVisibilityService, VisibilityService>();
            services.AddScoped<ITripOperationAuthorizationService, TripOperationAuthorizationService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<IBackupService, BackupService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IWhatsAppService, WhatsAppService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IMapService, MapService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddHostedService<RoleSeederHostedService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = GetRequiredJwtSetting(configuration, "Jwt:Key");
            var issuer = GetRequiredJwtSetting(configuration, "Jwt:Issuer");
            var audience = GetRequiredJwtSetting(configuration, "Jwt:Audience");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = securityKey,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            return services;
        }

        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

            services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

        private static string GetRequiredJwtSetting(IConfiguration configuration, string key)
        {
            var value = configuration[key]?.Trim();

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{key} is not configured");

            if (key == "Jwt:Key" && Encoding.UTF8.GetByteCount(value) < 32)
                throw new InvalidOperationException("Jwt:Key must be at least 32 bytes for HMAC SHA256");

            return value;
        }
    }
}
