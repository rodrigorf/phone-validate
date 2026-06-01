using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using PhoneValidate.Infra.Data;

namespace PhoneValidate.Integration.Tests;

/// <summary>
/// Boots the real API in-memory and swaps SQL Server for the EF Core
/// in-memory provider, so integration tests run without a database.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove every registration tied to the relational (SQL Server) provider,
            // otherwise EF Core sees two providers and throws at runtime.
            services.RemoveAll(typeof(DbContextOptions<PhoneValidateDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            services.RemoveAll<IDbContextOptionsConfiguration<PhoneValidateDbContext>>();

            services.AddDbContext<PhoneValidateDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDb"));
        });
    }
}
