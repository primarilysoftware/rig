using Rig.Api;
using Rig.Domain;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Rig.Sample.Domain.Todos;

namespace Rig.Sample.Web.Infrastructure;

public static class SerializationExtensions
{
    public static void ConfigureJsonOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    JsonTypeInfoResolverModifiers.AddPolymorphicType<AggregateRoot>(typeof(Todo).Assembly),
                    JsonTypeInfoResolverModifiers.AddPolymorphicType<DomainEvent>(typeof(Todo).Assembly),
                    JsonTypeInfoResolverModifiers.AddPrivateSetters
                }
            };
        });
    }
}
