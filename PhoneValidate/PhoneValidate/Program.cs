using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhoneValidate.Application.Service.Services;
using PhoneValidate.Application.Services.Interfaces;
using PhoneValidate.Domain.Service.Interfaces;
using PhoneValidate.Extensions;
using PhoneValidate.Infra.Data;
using PhoneValidate.Infra.Data.Repositories;
using PhoneValidate.Domain.Service.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Log highlight
builder.Logging.AddSimpleConsole(options =>
{
    options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
    options.SingleLine = false;
    options.IncludeScopes = true;
    options.TimestampFormat = "HH:mm:ss ";
});

builder.Services.AddControllers();

builder.Services.AddDbContext<PhoneValidateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//HealthCheck
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PhoneValidateDbContext>("database");

//Services
builder.Services.AddScoped<IRecipientService, RecipientService>();
builder.Services.AddScoped<IRecipientAppService, RecipientAppService>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

//Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

//Swagger
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();
await app.ApplyDatabaseMigrationsAsync<PhoneValidateDbContext>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PhoneValidateDbContext>();
    db.Database.Migrate();
}

object value = app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneValidate API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthCheckEndpoint();
app.MapControllers();

app.Run();
