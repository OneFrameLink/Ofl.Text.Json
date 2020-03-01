using System.Text.Json;

namespace Ofl.Text.Json
{
    public static class Utf8JsonReaderExtensions
    {
        internal static void ThrowEndOfJsonException() =>
            throw new JsonException($"Unexpectedly encountered end of JSON tokens when calling {nameof(Utf8JsonReader.Read)}.");

        public static void GuaranteedRead(this ref Utf8JsonReader reader)
        {
            // Read while the next token is not a comment.
            // TODO: Handle comments.
            if (!reader.Read()) ThrowEndOfJsonException();
        }

        public static void SkipObjectOrArray(this ref Utf8JsonReader reader)
        {
            // Get the token type.
            JsonTokenType startToken = reader.TokenType;

            // End token type.
            JsonTokenType endToken;

            // Switch.
            switch (startToken)
            {
                // Start object or start array, we're good.
                case JsonTokenType.StartArray:
                    // The end token is end array.
                    endToken = JsonTokenType.EndArray;
                    break;
                case JsonTokenType.StartObject:
                    // The end token is end object.
                    endToken = JsonTokenType.EndObject;
                    break;
                default:
                    // Throw.
                    throw new JsonException(
                        $"Expected a {nameof(JsonTokenType)} of {nameof(JsonTokenType.StartObject)} or {nameof(JsonTokenType.StartArray)}, got {startToken} instead.");
            }

            // The count.  There is one.
            int count = 1;

            // While there are items to read.
            while (reader.Read())
            {
                // Check the token type, if it is an end, then decrement.
                if (reader.TokenType == endToken)
                {
                    // Decrement.
                    // Is this 0?  Break.
                    if (--count == 0) break;
                }

                // Is this the start token?  Increment.
                if (reader.TokenType == startToken)
                    ++count;
            }

            // If the count is not 0, throw.
            if (count != 0 || !reader.Read())
                ThrowEndOfJsonException();
        }
    }
}
