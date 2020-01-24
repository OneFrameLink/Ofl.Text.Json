using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Ofl.Reflection;

namespace Ofl.Text.Json
{
    internal static class ExceptionExtensions
    {
        internal static readonly TypeInfo EnumerableExceptionTypeInfo = typeof(IEnumerable<Exception>).GetTypeInfo();

        internal static readonly TypeInfo ExceptionTypeInfo = typeof(Exception).GetTypeInfo();

        // TODO: Remove recursion.
        public static void WriteException(
            this Utf8JsonWriter writer,
            Exception? exception, 
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // If there is no exception, write null.
            if (exception == null)
            {
                // Write null.
                writer.WriteNullValue();

                // Write the end.
                writer.WriteEndObject();

                // Get out.
                return;
            }

            // Start writing the object.
            writer.WriteStartObject();

            // TODO: Serialize the Data property on
            // Exception, if there are values.                

            // Write properties.
            writer.WriteString(nameof(Exception.HelpLink), exception.HelpLink);
            writer.WriteNumber(nameof(Exception.HResult), exception.HResult);
            writer.WriteString(nameof(Exception.Message), exception.Message);
            writer.WriteString(nameof(Exception.Source), exception.Source);
            writer.WriteString(nameof(Exception.StackTrace), exception.StackTrace);

            // Need to write the type shim.  Write the meta property
            writer.WritePropertyName("$Meta");

            // Write all the type information.
            Type type = exception.GetType();

            // Write a new object.
            writer.WriteStartObject();

            // Write the type.
            writer.WritePropertyName("Type");

            // Write a new object.
            writer.WriteStartObject();

            // Write properties.
            writer.WriteString(nameof(Type.Namespace), type.Namespace);
            writer.WriteString(nameof(Type.FullName), type.FullName);
            writer.WriteString(nameof(Type.AssemblyQualifiedName), type.AssemblyQualifiedName);
            writer.WriteString(nameof(Type.Name), type.Name);

            // Close the type then the meta object.
            writer.WriteEndObject();
            writer.WriteEndObject();

            // Are there any properties which are assignable
            // to exception or IEnumerable<Exception>?
            // If so, write those.  Cycle through the properties.
            foreach (PropertyInfo propertyInfo in type.GetPropertiesWithPublicInstanceGetters())
            {
                // Is it assignable to exception?
                if (ExceptionTypeInfo.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    // Write the property name.
                    writer.WritePropertyName(propertyInfo.Name);

                    // Write the object.
                    writer.WriteException((Exception?) propertyInfo.GetValue(exception), options);
                }
                // If this is assignable to an IEnumerable<Exception> then
                // get that.
                else if (EnumerableExceptionTypeInfo.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    // Get the value.
                    var exceptions = (IEnumerable<Exception>?) propertyInfo.GetValue(exception);

                    // Write the property name.
                    writer.WritePropertyName(propertyInfo.Name);

                    // Write the exception array.
                    writer.WriteExceptionArray(exceptions, options);

                    // Continue.
                    continue;
                }
            }

            // Write the end of the object.
            writer.WriteEndObject();
        }

        private static void WriteExceptionArray(
            this Utf8JsonWriter writer,
            IEnumerable<Exception?>? exceptions,
            JsonSerializerOptions options
        )
        {
            // If the enumerable is null, write null and get out.
            if (exceptions == null)
            {
                // Write null.
                writer.WriteNullValue();

                // Get out.
                return;
            }

            // It's not null, write the start of the array.
            writer.WriteStartArray();

            // Cycle through the items.
            foreach (Exception? exception in exceptions)
                // Write the object.
                writer.WriteException(exception, options);

            // Write the end of the array and be done with it.
            writer.WriteEndArray();
        }
    }
}
