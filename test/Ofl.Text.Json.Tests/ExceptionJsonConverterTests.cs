using System;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class ExceptionJsonConverterTests
    {
        #region Helpers

        private static TestException CreateTestException()
        {
            // Creates the invalid operation exception.
            Exception CreateFillerException() => new InvalidOperationException(Guid.NewGuid().ToString());

            // Create an exception and return.
            return new TestException
            {
                ExceptionProperty = CreateFillerException(),
                ExceptionPropertyWithJsonPropertyAttribute = CreateFillerException(),
                ExceptionsProperty = new List<Exception> {
                    CreateFillerException(),
                    CreateFillerException()
                },
                AggregateException = new AggregateException(new List<Exception> {
                    CreateFillerException(),
                    CreateFillerException()
                })
            };
        }

        private static JsonDocument SerializeTestException() => JsonDocument.Parse(
            JsonSerializer.Serialize(
                CreateTestException(),
                new JsonSerializerOptions {
                    Converters = { new ExceptionJsonConverter() }
                }
            )
        );

        #endregion

        #region Tests

        [Fact]
        public void Test_SingleExceptionProperty()
        {
            // Setup.
            TestException setup = CreateTestException();

            // Serialize, then get the document.
            JToken token = JToken.FromObject(setup.TestException, setup.Serializer);

            // Start asserting.
            setup.Serializer.ContractResolver.AssertExceptionsEqual(setup.TestException.ExceptionProperty, token["ExceptionProperty"]);
        }

        [Fact]
        public void Test_EnumerableExceptionsProperty()
        {
            // Setup.
            TestException TestException = CreateTestException();

            // Create the token.
            JToken token = JToken.FromObject(setup.TestException, setup.Serializer);

            // Start asserting.
            setup.Serializer.ContractResolver.AssertExceptionsEqual(setup.TestException.ExceptionsProperty, token["ExceptionsProperty"]);
        }

        [Fact]
        public void Test_AggregateException()
        {
            // Setup.
            TestException TestException = CreateTestException();

            // Create the token.
            JToken token = JToken.FromObject(setup.TestException, setup.Serializer);

            // Start asserting.
            setup.Serializer.ContractResolver.AssertExceptionsEqual(setup.TestException.AggregateException, token["AggregateException"]);
        }

        [Fact]
        public void Test_ExceptionPropertyWithJsonPropertyAttribute()
        {
            // Setup.
            TestException TestException = CreateTestException();

            // Create the token.
            JToken token = JToken.FromObject(setup.TestException, setup.Serializer);

            // Start asserting.
            setup.Serializer.ContractResolver.AssertExceptionsEqual(setup.TestException.ExceptionPropertyWithJsonPropertyAttribute, token[TestException.ExceptionPropertyWithJsonPropertyAttributeName]);
        }

        #endregion
    }
}