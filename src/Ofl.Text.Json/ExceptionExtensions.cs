using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Ofl.Reflection;

namespace Ofl.Text.Json
{
    internal static class ExceptionExtensions
    {
        internal static readonly TypeInfo EnumerableExceptionTypeInfo = typeof(IEnumerable<Exception>).GetTypeInfo();

        internal static readonly TypeInfo ExceptionTypeInfo = typeof(Exception).GetTypeInfo();

        public static void WriteException(
            this Utf8JsonWriter writer,
            Exception? value, 
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Create the stack of exceptions to write.
            var stack = new Stack<(Exception? exception, Action? post)>();

            // Push the original exception on.
            stack.Push((value, null));

            // While there are items on the stack.
            while (stack.Any())
            {
                // Pop the item.
                var (exception, post) = stack.Pop();

                // If there is no exception, write null.
                if (exception == null)
                {
                    // Write null.
                    writer.WriteNullValue();

                    // If there is a post action, do it.
                    post?.Invoke();

                    // Continue.
                    continue;
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

                        // Write the property name.
                        writer.WritePropertyName(propertyInfo.Name);

                        // Push on the stack.
                        stack.Push((value, writer.WriteEndObject));

                        // Continue.
                        continue;
                    }

                    // If this is assignable to an IEnumerable<Exception> then
                    // get that.
                    if (EnumerableExceptionTypeInfo.IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        // Get the value.
                        IEnumerable<Exception>? exceptions = (IEnumerable<Exception>?) propertyInfo.GetValue(value);

                        // Write the property name.
                        writer.WritePropertyName(propertyInfo.Name);

                        // Write the exception array.
                        writer.WriteExceptionArray(exceptions, stack);

                        // Continue.
                        continue;
                    }
                }
                
                // Call the post action.
                post?.Invoke();
            }
        }

        private static void WriteExceptionArray(
            this Utf8JsonWriter writer,
            IEnumerable<Exception?>? enumerable,
            Stack<(Exception? exception, Action? post)> stack
        )
        {
            // If the enumerable is null, write null and get out.
            if (enumerable == null)
            {
                // Write null.
                writer.WriteNullValue();

                // Get out.
                return;
            }

            // It's not null, write the start of the array.
            writer.WriteStartArray();

            // Get the enumerator.
            using IEnumerator<Exception?> enumerator = enumerable.GetEnumerator();

            // Move to the first item.  If there are no items then just write
            // an empty array and get out.
            bool hasItems = enumerator.MoveNext();

            // If there are no items, write an empty array and be done
            // with it.
            if (!hasItems)
            {
                // Write the end of the array and be done with it.
                writer.WriteEndArray();

                // Get out.
                return;
            }

            // The previous item.
            Exception? previous = null, current = enumerator.Current;

            // Skip the first iteration.
            bool skip = true;

            // Cycle through the objects and add to the stack.
            do
            {
                // If skipping, flip.
                if (skip)
                {
                    // Flip.
                    skip = false;

                    // Contiune.
                    continue;
                }
                else
                    // Set the previous equal to the current.
                    previous = current;

                // Push the previous.
                stack.Push((previous, null));

                // Set the current.
                current = enumerator.Current;
            } while (enumerator.MoveNext());

            // The last one will close out the array.
            stack.Push((current, writer.WriteEndArray));
        }
    }
}
