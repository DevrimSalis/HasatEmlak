namespace HasatEmlak.Services
{
    public interface IAnalyticsService
    {
        Task TrackPageViewAsync(string page, string userAgent = null, string ipAddress = null);
        Task TrackPropertyViewAsync(int propertyId, string userAgent = null, string ipAddress = null);
        Task TrackSearchAsync(string searchTerm, int resultCount, string userAgent = null);
        Task<Dictionary<string, object>> GetDashboardAnalyticsAsync();
    }
}
