using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json.Tests
{
    internal class TestException : Exception
    {
        #region Constructor

        public TestException(
            Exception exception,
            IReadOnlyCollection<Exception> exceptions,
            Exception exceptionPropertyWithJsonPropertyAttribute,
            AggregateException aggregateException
        )
        {
            // Assign values.
            ExceptionProperty = exception;
            ExceptionsProperty = exceptions;
            ExceptionPropertyWithJsonPropertyAttribute = exceptionPropertyWithJsonPropertyAttribute;
            AggregateException = aggregateException;
        }

        #endregion

        #region Instance, read-only state

        public Exception ExceptionProperty { get; }

        public IReadOnlyCollection<Exception> ExceptionsProperty { get; }

        internal const string ExceptionPropertyWithJsonPropertyAttributeName = "MyException";

        [JsonPropertyName(ExceptionPropertyWithJsonPropertyAttributeName)]
        public Exception ExceptionPropertyWithJsonPropertyAttribute { get; }

        public AggregateException AggregateException { get; }

        #endregion
    }
}