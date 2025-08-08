using HasatEmlak.Data;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnalyticsService> _logger;
        private readonly ICacheService _cache;

        public AnalyticsService(ApplicationDbContext context, ILogger<AnalyticsService> logger, ICacheService cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task TrackPageViewAsync(string page, string userAgent = null, string ipAddress = null)
        {
            try
            {
                // Bu örnekte basit logging yapıyoruz
                // Gerçek uygulamada ayrı bir analytics tablosu oluşturabilirsiniz
                _logger.LogInformation($"Page view: {page}, IP: {ipAddress}, UA: {userAgent}");

                // Cache'de sayaçları güncelleyebiliriz
                var key = $"pageview_{page}_{DateTime.Now:yyyy-MM-dd}";
                var currentCount = await _cache.GetAsync<int>(key);
                await _cache.SetAsync(key, currentCount + 1, TimeSpan.FromDays(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking page view");
            }
        }

        public async Task TrackPropertyViewAsync(int propertyId, string userAgent = null, string ipAddress = null)
        {
            try
            {
                _logger.LogInformation($"Property view: {propertyId}, IP: {ipAddress}");

                var key = $"property_view_{propertyId}_{DateTime.Now:yyyy-MM-dd}";
                var currentCount = await _cache.GetAsync<int>(key);
                await _cache.SetAsync(key, currentCount + 1, TimeSpan.FromDays(7));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking property view");
            }
        }

        public async Task TrackSearchAsync(string searchTerm, int resultCount, string userAgent = null)
        {
            try
            {
                _logger.LogInformation($"Search: '{searchTerm}' ({resultCount} results)");

                var key = $"search_{DateTime.Now:yyyy-MM-dd}";
                var searches = await _cache.GetAsync<List<string>>(key) ?? new List<string>();
                searches.Add(searchTerm);
                await _cache.SetAsync(key, searches, TimeSpan.FromDays(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking search");
            }
        }

        public async Task<Dictionary<string, object>> GetDashboardAnalyticsAsync()
        {
            try
            {
                var analytics = new Dictionary<string, object>();

                // Temel istatistikler
                analytics["TotalProperties"] = await _context.Properties.CountAsync();
                analytics["ActiveProperties"] = await _context.Properties.CountAsync(p => p.IsActive);
                analytics["TotalMessages"] = await _context.ContactRequests.CountAsync();
                analytics["UnreadMessages"] = await _context.ContactRequests.CountAsync(c => !c.IsRead);

                // Son 30 günlük veriler
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                analytics["PropertiesLast30Days"] = await _context.Properties
                    .CountAsync(p => p.CreatedDate >= thirtyDaysAgo);

                analytics["MessagesLast30Days"] = await _context.ContactRequests
                    .CountAsync(c => c.CreatedDate >= thirtyDaysAgo);

                // Popüler şehirler
                analytics["PopularCities"] = await _context.Properties
                    .Where(p => p.IsActive)
                    .GroupBy(p => p.City.Name)
                    .Select(g => new { City = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToListAsync();

                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard analytics");
                return new Dictionary<string, object>();
            }
        }
    }
}