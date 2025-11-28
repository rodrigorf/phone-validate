using Microsoft.EntityFrameworkCore;
using PhoneValidate.Infra.Data;
using PhoneValidation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<PhoneValidateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//HealthCheck
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PhoneValidateDbContext>("database");

builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

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
