using System;
using System.Collections.Generic;
using System.Text.Json;
using Ofl.Linq;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    internal static class AssertExtensions
    {
        public static void AssertExceptionsEqual(
            IEnumerable<Exception> expected, 
            JsonElement actual,
            string property
        )
        {
            // Cycle through the exceptions, compare to actual.  Zip.
            foreach (var pair in actual.GetProperty(property).EnumerateArray()
                .ZipChecked(expected, (element, exception) => (element, exception)))
                // Assert.
                AssertExceptionsEqual(pair.exception, pair.element);
        }

        public static void AssertExceptionsEqual(
            Exception expected, 
            JsonElement actual,
            string? property = null
        )
        {
            // Get the element.  If there is a property, use that
            // otherwise just use the element.
            if (!string.IsNullOrWhiteSpace(property))
                // Try and get the property.
                Assert.True(actual.TryGetProperty(property, out actual),
                    $"Could not find the property {property} on the {nameof(JsonElement)} passed in.");

            // Start comparing.
            Assert.Equal(expected.HResult, actual.GetProperty(nameof(expected.HResult)).GetInt32());
            Assert.Equal(expected.HelpLink, actual.GetProperty(nameof(expected.HelpLink)).GetString());
            Assert.Equal(expected.Source, actual.GetProperty(nameof(expected.Source)).GetString());
            Assert.Equal(expected.StackTrace, actual.GetProperty(nameof(expected.StackTrace)).GetString());
            Assert.Equal(expected.Message, actual.GetProperty(nameof(expected.Message)).GetString());

            // Get type token and value.
            JsonElement type = actual.GetProperty("$Meta").GetProperty("Type");

            // Get the values.
            Assert.Equal(expected.GetType().Namespace, type.GetProperty("Namespace").GetString());
            Assert.Equal(expected.GetType().FullName, type.GetProperty("FullName").GetString());
            Assert.Equal(expected.GetType().AssemblyQualifiedName, type.GetProperty("AssemblyQualifiedName").GetString());
            Assert.Equal(expected.GetType().Name, type.GetProperty("Name").GetString());
        }
    }
}
