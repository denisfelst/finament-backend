using Finament.Api.Middleware;
using Finament.Application.Infrastructure;
using Finament.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddControllers();

// --- SWAGGER ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {  Title = "Finament.Api", Version = "v1", Description = "The Finament API" });
});

builder.Services.AddDbContext<FinamentDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var  frontendcors = "_frontendCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: frontendcors,
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddScoped<IFinamentDbContext, FinamentDbContext>();

// === BUILD ===
var app = builder.Build();

app.UseCors(frontendcors);

AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finament.Api v1");
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();