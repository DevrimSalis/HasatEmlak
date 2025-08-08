namespace HasatEmlak.Services
{
    public interface ISeoService
    {
        string GenerateMetaTitle(string title, string cityName = null, string categoryName = null);
        string GenerateMetaDescription(string description, string price = null, string location = null);
        string GenerateSlug(string text);
        string GenerateCanonicalUrl(string path);
        Dictionary<string, string> GenerateStructuredData(object data);
    }
}
