using HasatEmlak.Services;

namespace HasatEmlak.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHasatEmlakServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISeoService, SeoService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ICacheService, CacheService>();

            // Memory Cache
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1000; // Max 1000 entries
            });

            // Response Compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
                options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
            });

            return services;
        }
    }
}