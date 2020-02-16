using System;
using System.Text.Json;

namespace Ofl.Text.Json
{
    public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        #region Constructor

        public SnakeCaseJsonNamingPolicy(
            bool lowercaseParts
        )
        {
            // Assign values.
            _lowercaseParts = lowercaseParts;
        }

        #endregion

        #region Instance, read-only state

        private readonly bool _lowercaseParts;

        #endregion

        #region Overrides

        public override string ConvertName(string name)
        {
            // Validate parameters.
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            // Allocate locally.  Allocate two times the length
            // of the string that came in.
            Span<char> buffer = stackalloc char[name.Length * 2];

            // The current index in the span.
            int index = 0;

            // Cycle through the characters.
            foreach (char c in name)
            {
                // Is this upper case?
                if (char.IsUpper(c))
                {
                    // If this is not the first character, add an
                    // underscore.
                    if (index > 0)
                        buffer[index++] = '_';

                    // Lowercase if lowercasing the parts.
                    buffer[index++] = _lowercaseParts
                        ? char.ToLower(c)
                        : c;
                }
                else
                    // Add the buffer.
                    buffer[index++] = c;
            }

            // Slice the buffer.
            buffer = buffer.Slice(0, index);

            // Return a new string.
            return new string(buffer);
        }

        #endregion
    }
}
