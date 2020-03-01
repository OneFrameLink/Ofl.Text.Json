using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class ReadOnlyDictionaryJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));

            // Is this generic.
            Type? generic = typeToConvert.IsGenericType
                ? typeToConvert.GetGenericTypeDefinition() 
                : null;

            // If not generic, get out.
            if (generic == null) return false;

            // Is this a read-only dictionary?  If so, return that.
            if (generic == typeof(ReadOnlyDictionary<,>))
                // True.
                return true;

            // Is it IReadOnlyDictionary<,>?
            if (generic == typeof(IReadOnlyDictionary<,>))
                // True.
                return true;
            
            // False.
            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (typeToConvert.GenericTypeArguments.Length != 2)
                throw new InvalidOperationException($"Expected two type parameters from the type {typeToConvert.FullName}, found {typeToConvert.GenericTypeArguments.Length}.");

            // There are two generic parameters.  Get them.
            Type keyType = typeToConvert.GenericTypeArguments[0];
            Type valueType = typeToConvert.GenericTypeArguments[1];

            // Create the type.
            Type converterType = typeof(ReadOnlyDictionaryJsonConverter<,>).MakeGenericType(keyType, valueType);

            // Create the instance and return.
            return (JsonConverter) Activator.CreateInstance(converterType);
        }
    }
}
