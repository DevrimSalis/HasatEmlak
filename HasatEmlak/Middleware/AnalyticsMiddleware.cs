using HasatEmlak.Services;

namespace HasatEmlak.Middleware
{
    public class AnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AnalyticsMiddleware> _logger;

        public AnalyticsMiddleware(RequestDelegate next, ILogger<AnalyticsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAnalyticsService analyticsService)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await _next(context);

                stopwatch.Stop();

                // Analytics tracking (non-blocking)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var path = context.Request.Path.Value;
                        var userAgent = context.Request.Headers["User-Agent"].ToString();
                        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                        // Sadece GET isteklerini ve normal sayfaları track et
                        if (context.Request.Method == "GET" &&
                            !path.StartsWith("/css") &&
                            !path.StartsWith("/js") &&
                            !path.StartsWith("/images") &&
                            !path.StartsWith("/lib"))
                        {
                            await analyticsService.TrackPageViewAsync(path, userAgent, ipAddress);
                        }

                        // Property detay sayfalarını özel olarak track et
                        if (path.StartsWith("/Property/Details/"))
                        {
                            if (int.TryParse(path.Split('/').Last(), out int propertyId))
                            {
                                await analyticsService.TrackPropertyViewAsync(propertyId, userAgent, ipAddress);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in analytics tracking");
                    }
                });

                // Performance logging
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    _logger.LogWarning($"Slow request: {context.Request.Path} took {stopwatch.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing request: {context.Request.Path}");
                throw;
            }
        }
    }
}