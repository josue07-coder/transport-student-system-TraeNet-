using Transport.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

//  Extensions
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddMediatRServices();
builder.Services.AddSwaggerDocs();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocs();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();