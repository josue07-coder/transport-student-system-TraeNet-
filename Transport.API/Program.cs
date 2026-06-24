using Transport.API.Extensions;
using Transport.API.Conventions;
using Transport.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//  Extensions
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddMediatRServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocs();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new ApiVersionRouteConvention());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocs();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ApiVersionHeaderMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

//app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
