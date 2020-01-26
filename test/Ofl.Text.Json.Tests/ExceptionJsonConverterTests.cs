using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class ExceptionJsonConverterTests
    {
        #region Helpers

        private static TestException CreateTestException()
        {
            // Creates the invalid operation exception.
            static Exception CreateFillerException() => new InvalidOperationException(Guid.NewGuid().ToString());

            // Create an exception and return.
            return new TestException(
                CreateFillerException(),
                new List<Exception> {
                    CreateFillerException(),
                    CreateFillerException()
                },
                CreateFillerException(),
                new AggregateException(new List<Exception> {
                    CreateFillerException(),
                    CreateFillerException()
                })
            );
        }

        private static (TestException exception, JsonElement element) Setup(
            JsonNamingPolicy? jsonNamingPolicy = null
        ) {
            // Create the exception.
            TestException exception = CreateTestException();

            // Serialize.
            string json = JsonSerializer.Serialize(exception,
                new JsonSerializerOptions {
                    Converters = { new ExceptionJsonConverter() },
                    PropertyNamingPolicy = jsonNamingPolicy
                }
            );

            // Parse.
            JsonDocument document = JsonDocument.Parse(json);

            // Return the pair.
            return (exception, document.RootElement);
        }

        #endregion

        #region Tests

        [Fact]
        public void Test_SingleExceptionProperty()
        {
            // Setup.
            (TestException expected, JsonElement actual) = Setup();

            // Start asserting.
            AssertExtensions.AssertExceptionsEqual(expected.ExceptionProperty, actual, nameof(TestException.ExceptionProperty));
        }

        [Fact]
        public void Test_EnumerableExceptionsProperty()
        {
            // Setup.
            (TestException expected, JsonElement actual) = Setup();

            // Start asserting.
            AssertExtensions.AssertExceptionsEqual(expected.ExceptionsProperty, actual, nameof(TestException.ExceptionsProperty));
        }

        [Fact]
        public void Test_AggregateException()
        {
            // Setup.
            (TestException expected, JsonElement actual) = Setup();

            // Start asserting.
            AssertExtensions.AssertExceptionsEqual(expected.AggregateException, actual, nameof(TestException.AggregateException));
        }

        [Fact]
        public void Test_ExceptionPropertyWithJsonPropertyAttribute()
        {
            // Setup.
            (TestException expected, JsonElement actual) = Setup();

            // Start asserting.
            AssertExtensions.AssertExceptionsEqual(expected.ExceptionPropertyWithJsonPropertyAttribute, actual, TestException.ExceptionPropertyWithJsonPropertyAttributeName);
        }

        [Fact]
        public void Test_ExceptionProperty_Respects_NamingPolicy()
        {
            // Setup.
            (TestException exception, JsonElement document) = Setup(JsonNamingPolicy.CamelCase);

            // Get the expected name.
            string expected = JsonNamingPolicy.CamelCase.ConvertName(nameof(exception.ExceptionProperty));

            // Try and get the property
            Assert.True(document.TryGetProperty(expected, out _),
                $"Could not find the expected property {expected} on the {nameof(JsonElement)}.");
        }

        [Fact]
        public void Test_Data_Is_Serialized()
        {
            // Setup.
            (TestException exception, JsonElement document) = Setup();

            // The name.
            const string expected = nameof(exception.Data);

            // Try and get the property
            Assert.True(document.TryGetProperty(expected, out _),
                $"Could not find the expected property {expected} on the {nameof(JsonElement)}.");
        }

        #endregion
    }
}