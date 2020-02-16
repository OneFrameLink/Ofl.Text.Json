using System;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class SnakeCaseNamingPolicyTests
    {
        [Theory]
        [InlineData("Hello", "Hello", false)]
        [InlineData("Hello", "hello", true)]
        [InlineData("HelloWorld", "Hello_World", false)]
        [InlineData("HelloWorld", "hello_world", true)]
        public void Test_Convert(string test, string expected, bool lowercaseParts)
        {
            // Validate parameters.
            if (string.IsNullOrWhiteSpace(test))
                throw new ArgumentNullException(nameof(test));
            if (string.IsNullOrWhiteSpace(expected))
                throw new ArgumentNullException(nameof(expected));

            // Create the policy.
            var policy = new SnakeCaseJsonNamingPolicy(lowercaseParts);

            // Convert.
            string actual = policy
                .ConvertName(test);

            // They are equal.
            Assert.Equal(expected, actual);
        }
    }
}
