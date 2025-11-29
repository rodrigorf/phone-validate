using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace PhoneValidate.Extensions;

public static class HealthCheckExtensions
{
    public static IEndpointRouteBuilder MapHealthCheckEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/healthystatus", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        exception = e.Value.Exception?.Message,
                        data = e.Value.Data,
                        duration = e.Value.Duration.TotalMilliseconds
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
            }
        });

        return endpoints;
    }
}