using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Rig.CosmosDb;

public static class WebApplicationBuilderExtensions
{
    public static void RigCosmosDb(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddSingleton(services =>
        {
            var jsonOptions = services.GetRequiredService<IOptions<JsonOptions>>();
            var config = new CosmosClientOptions
            {
                Serializer = new CosmosSystemTextJsonSerializer(jsonOptions.Value.SerializerOptions),
                ConnectionMode = ConnectionMode.Gateway
            };

            if (builder.Environment.IsDevelopment())
            {
                config.HttpClientFactory = () =>
                {
                    return new HttpClient(new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    });
                };
            }

            return new CosmosClient(connectionString, config);
        });
    }
}
