using Application.Exceptions;
using Application.Mappings;
using Infrastructure;
using Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sports Depth Chart Api", Version = "v1" });
    c.IncludeXmlComments(Assembly.GetExecutingAssembly());
});

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Command.CreateGameHandler).Assembly));

// Sqlite
var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");
builder.Services.AddDbContext<PlayerDepthsContext>(options =>
    options.UseSqlite(connectionString));

// Error Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


// AutoMapper
builder.Services.AddAutoMapper(typeof(EntityMappingProfile));
builder.Services.AddAutoMapper(typeof(Program));

// Dependency Injection
builder.Services.AddTransient<IDbHandler, DbHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.Run();
