using System.Text;
using System.Text.RegularExpressions;

namespace HasatEmlak.Helpers
{
    public static class PerformanceHelper
    {
        public static string OptimizeImagePath(string imagePath, int? width = null, int? height = null)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "/images/placeholder.jpg";

            // WebP formatını destekleyen tarayıcılar için optimizasyon
            var extension = Path.GetExtension(imagePath);
            var basePath = imagePath.Replace(extension, "");

            if (width.HasValue && height.HasValue)
                return $"{basePath}_{width}x{height}.webp";
            else if (width.HasValue)
                return $"{basePath}_w{width}.webp";

            return imagePath;
        }

        public static string CompressHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // Gereksiz boşlukları kaldır
            html = Regex.Replace(html, @"\s+", " ");
            html = Regex.Replace(html, @">\s+<", "><");

            return html.Trim();
        }

        public static string GenerateCacheKey(string prefix, params object[] parameters)
        {
            var key = new StringBuilder(prefix);

            foreach (var param in parameters)
            {
                if (param != null)
                    key.Append($"_{param}");
            }

            return key.ToString().ToLowerInvariant();
        }
    }
}