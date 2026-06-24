using System.Diagnostics;

namespace Transport.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = ResolveCorrelationId(context);
            context.TraceIdentifier = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["TraceId"] = Activity.Current?.TraceId.ToString() ?? correlationId
            });

            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }

        private static string ResolveCorrelationId(HttpContext context)
        {
            var headerValue = context.Request.Headers[HeaderName].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(headerValue))
                return headerValue.Length <= 100 ? headerValue : headerValue[..100];

            return Guid.NewGuid().ToString("N");
        }
    }
}
