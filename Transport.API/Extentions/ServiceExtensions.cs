using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Transport.Application.Interfaces;
using Transport.Infrastructure.Persistence.Repositories;
using Transport.Infrastructure.Repositories;


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

            return services;
        }

        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            return services;
        }
    }
}