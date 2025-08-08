using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HasatEmlak.Services
{
    public class SeoService : ISeoService
    {
        private readonly IConfiguration _configuration;

        public SeoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateMetaTitle(string title, string cityName = null, string categoryName = null)
        {
            var siteName = _configuration["SiteSettings:SiteName"] ?? "HasatEmlak";

            var metaTitle = new StringBuilder(title);

            if (!string.IsNullOrEmpty(cityName))
                metaTitle.Append($" - {cityName}");

            if (!string.IsNullOrEmpty(categoryName))
                metaTitle.Append($" - {categoryName}");

            metaTitle.Append($" | {siteName}");

            // SEO için 60 karakter sınırı
            var result = metaTitle.ToString();
            return result.Length > 60 ? result.Substring(0, 57) + "..." : result;
        }

        public string GenerateMetaDescription(string description, string price = null, string location = null)
        {
            var metaDesc = new StringBuilder();

            if (!string.IsNullOrEmpty(description))
            {
                // HTML etiketlerini temizle
                var cleanDesc = Regex.Replace(description, "<.*?>", string.Empty);
                metaDesc.Append(cleanDesc.Length > 100 ? cleanDesc.Substring(0, 100) : cleanDesc);
            }

            if (!string.IsNullOrEmpty(price))
                metaDesc.Append($" Fiyat: {price} ₺.");

            if (!string.IsNullOrEmpty(location))
                metaDesc.Append($" Konum: {location}.");

            metaDesc.Append(" HasatEmlak'ta güvenilir emlak ilanları.");

            // SEO için 160 karakter sınırı
            var result = metaDesc.ToString();
            return result.Length > 160 ? result.Substring(0, 157) + "..." : result;
        }

        public string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Türkçe karakterleri değiştir
            text = text.Replace("ç", "c").Replace("Ç", "C")
                      .Replace("ğ", "g").Replace("Ğ", "G")
                      .Replace("ı", "i").Replace("I", "I")
                      .Replace("İ", "i").Replace("ö", "o")
                      .Replace("Ö", "O").Replace("ş", "s")
                      .Replace("Ş", "S").Replace("ü", "u")
                      .Replace("Ü", "U");

            // Küçük harfe çevir
            text = text.ToLowerInvariant();

            // Özel karakterleri kaldır
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Birden fazla boşluğu tek boşluğa çevir
            text = Regex.Replace(text, @"\s+", " ").Trim();

            // Boşlukları tire ile değiştir
            text = Regex.Replace(text, @"\s", "-");

            // Birden fazla tireyi tek tire yap
            text = Regex.Replace(text, @"-+", "-");

            // Baş ve sondaki tireleri kaldır
            text = text.Trim('-');

            return text;
        }

        public string GenerateCanonicalUrl(string path)
        {
            var baseUrl = _configuration["SiteSettings:BaseUrl"] ?? "https://localhost";
            return $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        }

        public Dictionary<string, string> GenerateStructuredData(object data)
        {
            var structuredData = new Dictionary<string, string>();

            if (data is HasatEmlak.Models.Entities.Property property)
            {
                var schema = new
                {
                    context = "https://schema.org",
                    type = "RealEstateListing",
                    name = property.Title,
                    description = property.Description,
                    price = new
                    {
                        type = "PriceSpecification",
                        price = property.Price,
                        priceCurrency = "TRY"
                    },
                    address = new
                    {
                        type = "PostalAddress",
                        addressLocality = property.City?.Name,
                        addressRegion = property.District?.Name,
                        addressCountry = "TR"
                    },
                    floorSize = new
                    {
                        type = "QuantitativeValue",
                        value = property.Area,
                        unitCode = "MTK"
                    },
                    numberOfRooms = property.RoomCount,
                    numberOfBathroomsTotal = property.BathroomCount
                };

                structuredData["application/ld+json"] = JsonSerializer.Serialize(schema, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }

            return structuredData;
        }
    }
}