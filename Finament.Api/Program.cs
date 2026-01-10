using System.Text;
using Finament.Api.Middleware;
using Finament.Api.Security;
using Finament.Application.Infrastructure;
using Finament.Application.Interfaces.Api;
using Finament.Application.Security;
using Finament.Application.Services.Auth;
using Finament.Application.Services.Category;
using Finament.Application.Services.Expense;
using Finament.Application.Services.Setting;
using Finament.Application.Services.User;
using Finament.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



// Application services
builder.Services.AddScoped<IFinamentDbContext, FinamentDbContext>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Auth
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddHttpContextAccessor();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// === BUILD ===
var app = builder.Build();

app.UseCors(frontendcors);

app.UseAuthentication();
app.UseAuthorization();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinamentDbContext>();
    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("✅ Conectado correctamente a Supabase");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error de conexión a la DB");
        Console.WriteLine(ex.Message);
    }
}


app.Run();