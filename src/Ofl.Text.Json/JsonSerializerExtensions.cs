using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ofl.Text.Json
{
    public static class JsonSerializerExtensions
    {
        public static async Task<MemoryStream> SerializeToMemoryStreamAsync<TRequest>(
            TRequest request,
            CancellationToken cancellationToken
        )
        {
            // Validate parameters.
            if (request == null) throw new ArgumentNullException(nameof(request));

            // Create the stream.
            var ms = new MemoryStream();

            // Serialize to the stream async.
            await JsonSerializer
                .SerializeAsync(ms, request, null, cancellationToken)
                .ConfigureAwait(false);

            // Move to the beginning of the stream.
            ms.Seek(0, SeekOrigin.Begin);

            // Return the stream.
            return ms;
        }

        public static async Task<MemoryStream> SerializeToMemoryStreamAsync<TRequest>(
            TRequest request,
            JsonSerializerOptions options,
            CancellationToken cancellationToken
        )
        {
            // Validate parameters.
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Create the stream.
            var ms = new MemoryStream();

            // Serialize to the stream async.
            await JsonSerializer
                .SerializeAsync(ms, request, options, cancellationToken)
                .ConfigureAwait(false);

            // Move to the beginning of the stream.
            ms.Seek(0, SeekOrigin.Begin);

            // Return the stream.
            return ms;
        }
    }
}
