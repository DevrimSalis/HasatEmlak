using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.ViewModels
{
    public class SearchViewModel
    {
        public int? CategoryId { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? NeighborhoodId { get; set; }

        [Display(Name = "En Az Fiyat")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "En Çok Fiyat")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "En Az Alan (m²)")]
        public int? MinArea { get; set; }

        [Display(Name = "En Çok Alan (m²)")]
        public int? MaxArea { get; set; }

        [Display(Name = "Oda Sayısı")]
        public int? RoomCount { get; set; }

        [Display(Name = "Arama Kelimesi")]
        public string Keywords { get; set; }

        public string SortBy { get; set; } = "date_desc";
    }
}
