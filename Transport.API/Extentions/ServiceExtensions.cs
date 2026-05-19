using FluentValidation;
using MediatR;
using Transport.Application;
using Transport.Application.Common.Behaviors;
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
            services.AddScoped<ISectorRepository, SectorRepository>();

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
    }
}
