using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ofl.Reflection;
using System.Text.Json;

namespace Ofl.Serialization.Json.Newtonsoft
{
    internal static class ExceptionExtensions
    {
        internal static readonly TypeInfo EnumerableExceptionTypeInfo = typeof(IEnumerable<Exception>).GetTypeInfo();

        internal static readonly TypeInfo ExceptionTypeInfo = typeof(Exception).GetTypeInfo();

        internal static void WriteException(
            this Utf8JsonWriter writer,
            Exception? value, 
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Create the stack of exceptions to write.
            var stack = new Stack<Exception?>();

            // Push the original exception on.
            stack.Push(value);

            // While there are items on the stack.
            while (stack.Any())
            {
                // Pop the item.
                value = stack.Pop();

                // If there is no exception, write null and be done with it.
                if (value == null)
                {
                    // Write null.
                    writer.WriteNullValue();

                    // Continue.
                    continue;
                }

                // Write object values.
                writer.WriteStartObject();

                // TODO: Serialize the Data property on
                // Exception, if there are values.                

                // Write properties.
                writer.WriteString(nameof(Exception.HelpLink), value.HelpLink);
                writer.WriteNumber(nameof(Exception.HResult), value.HResult);
                writer.WriteString(nameof(Exception.Message), value.Message);
                writer.WriteString(nameof(Exception.Source), value.Source);
                writer.WriteString(nameof(Exception.StackTrace), value.StackTrace);

                // Need to write the type shim.  Write the meta property
                writer.WritePropertyName("$Meta");

                // Write all the type information.
                Type type = value.GetType();

                // Write a new object.
                writer.WriteStartObject();

                // Write properties.
                writer.WriteString(nameof(Type.Namespace), type.Namespace);
                writer.WriteString(nameof(Type.FullName), type.FullName);
                writer.WriteString(nameof(Type.AssemblyQualifiedName), type.AssemblyQualifiedName);
                writer.WriteString(nameof(Type.Name), type.Name);

                // Close the meta object.
                writer.WriteEndObject();

                // Are there any properties which are assignable
                // to exception or IEnumerable<Exception>?
                // If so, write those.  Cycle through the properties.
                foreach (PropertyInfo propertyInfo in type.GetPropertiesWithPublicInstanceGetters())
                {
                    // Is it assignable to exception?
                    if (ExceptionTypeInfo.IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        // Get the value.
                        value = (Exception?) propertyInfo.GetValue(value);
                    }
                }

                // Close the object.
                writer.WriteEndObject();
            }
        }
    }
}
