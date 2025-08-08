using HasatEmlak.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HasatEmlak.Extensions
{
    public static class HtmlExtensions
    {
        public static IHtmlContent MetaTitle(this IHtmlHelper html, string title, string cityName = null, string categoryName = null)
        {
            var seoService = html.ViewContext.HttpContext.RequestServices.GetService<ISeoService>();
            var metaTitle = seoService?.GenerateMetaTitle(title, cityName, categoryName) ?? title;

            return new HtmlString($"<title>{metaTitle}</title>");
        }

        public static IHtmlContent MetaDescription(this IHtmlHelper html, string description, string price = null, string location = null)
        {
            var seoService = html.ViewContext.HttpContext.RequestServices.GetService<ISeoService>();
            var metaDesc = seoService?.GenerateMetaDescription(description, price, location) ?? description;

            return new HtmlString($"<meta name=\"description\" content=\"{metaDesc}\">");
        }

        public static IHtmlContent CanonicalUrl(this IHtmlHelper html, string path = null)
        {
            var seoService = html.ViewContext.HttpContext.RequestServices.GetService<ISeoService>();
            var currentPath = path ?? html.ViewContext.HttpContext.Request.Path;
            var canonicalUrl = seoService?.GenerateCanonicalUrl(currentPath) ?? currentPath;

            return new HtmlString($"<link rel=\"canonical\" href=\"{canonicalUrl}\">");
        }

        public static IHtmlContent StructuredData(this IHtmlHelper html, object data)
        {
            var seoService = html.ViewContext.HttpContext.RequestServices.GetService<ISeoService>();
            var structuredData = seoService?.GenerateStructuredData(data);

            if (structuredData != null && structuredData.ContainsKey("application/ld+json"))
            {
                return new HtmlString($"<script type=\"application/ld+json\">{structuredData["application/ld+json"]}</script>");
            }

            return HtmlString.Empty;
        }

        public static string FormatPrice(this IHtmlHelper html, decimal price)
        {
            if (price >= 1000000)
                return $"{price / 1000000:F1}M ₺";
            else if (price >= 1000)
                return $"{price / 1000:F0}K ₺";
            else
                return $"{price:N0} ₺";
        }

        public static string TimeAgo(this IHtmlHelper html, DateTime date)
        {
            var timeSpan = DateTime.Now - date;

            if (timeSpan.Days > 365)
                return $"{timeSpan.Days / 365} yıl önce";
            else if (timeSpan.Days > 30)
                return $"{timeSpan.Days / 30} ay önce";
            else if (timeSpan.Days > 0)
                return $"{timeSpan.Days} gün önce";
            else if (timeSpan.Hours > 0)
                return $"{timeSpan.Hours} saat önce";
            else if (timeSpan.Minutes > 0)
                return $"{timeSpan.Minutes} dakika önce";
            else
                return "Az önce";
        }
    }
}