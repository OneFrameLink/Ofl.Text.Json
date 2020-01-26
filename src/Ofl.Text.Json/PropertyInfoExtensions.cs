using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public static class PropertyInfoExtensions
    {
        public static string GetPropertyName(
            this PropertyInfo property,
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Check to see if there is an attribute on the property
            // for naming.
            JsonPropertyNameAttribute? attribute = property
                .GetCustomAttribute<JsonPropertyNameAttribute>(true);

            // Is there an attribute?  If so, use that name.
            if (attribute != null) return attribute.Name;

            // Get the property name, run through the
            // converter.
            return options.PropertyNamingPolicy?.ConvertName(property.Name)
                ?? property.Name;
        }
    }
}
