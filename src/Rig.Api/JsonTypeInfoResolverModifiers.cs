using System.Reflection;
using System.Text.Json.Serialization.Metadata;

namespace Rig.Api;

public class JsonTypeInfoResolverModifiers
{
    public static void AddPrivateSetters(JsonTypeInfo jsonTypeInfo)
    {
        foreach (var property in jsonTypeInfo.Properties)
        {
            if (property.Get is not null && property.Set is null)
            {
                var propertyInfo = jsonTypeInfo.Type.GetProperty(property.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo is null)
                {
                    continue;
                }

                property.Set = propertyInfo.SetValue;
            }
        }
    }

    public static Action<JsonTypeInfo> AddPolymorphicType<T>(params Assembly[] sourceAssemblies)
    {
        return jsonTypeInfo =>
        {
            if (jsonTypeInfo.Type == typeof(T))
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions();

                var targetTypes = sourceAssemblies
                    .SelectMany(assembly => assembly.GetExportedTypes())
                    .Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(T)))
                    .Select(type => new JsonDerivedType(type, type.FullName ?? type.Name))
                    .ToList();

                foreach (var type in targetTypes)
                {
                    jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(type);
                }
            }
        };
    }
}
