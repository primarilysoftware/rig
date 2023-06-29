using Rig.Api;
using Rig.Domain;

namespace Rig.Sample.Web.Api;

public static class ApiExtensions
{
    public static void ConfigureApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Transient;
        });

        builder.Services.AddProblemDetails(options =>
        {
            options.MapExceptionToStatusCode<NotFoundException>(StatusCodes.Status404NotFound);
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();
    }

    public static void MapApi(this WebApplication app)
    {
        app.MapGroup("api")
            .WithOpenApi()
            .MapTodoEndpoints();

        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
