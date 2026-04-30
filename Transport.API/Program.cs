using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using Transport.Infrastructure.Persistence.Context;
using Transport.Infrastructure.Persistence.Repositories;
using Transport.Application.Interfaces;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Features.Students.Commands.AssignStudentToRoute;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Features.Students.Commands.AssignStudentToRoute;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Features.Students.Commands.AssignStudentToRoute;

var builder = WebApplication.CreateBuilder(args);

//  DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

//  MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateStudentCommand).Assembly));

//  FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentCommand>();

//  Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IRouteAssignmentRepository, RouteAssignmentRepository>();

//  Controllers
builder.Services.AddControllers();

//  Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//  Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();