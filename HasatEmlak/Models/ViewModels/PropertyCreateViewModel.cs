using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.ViewModels
{
    public class PropertyCreateViewModel
    {
        [Required(ErrorMessage = "Başlık gereklidir")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        [Display(Name = "İlan Başlığı")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama gereklidir")]
        [Display(Name = "İlan Açıklaması")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir")]
        [Display(Name = "Fiyat (₺)")]
        [Range(0, 999999999, ErrorMessage = "Geçerli bir fiyat giriniz")]
        public decimal Price { get; set; }

        [Display(Name = "Alan (m²)")]
        [Range(1, 10000, ErrorMessage = "Alan 1-10000 m² arasında olmalıdır")]
        public int? Area { get; set; }

        [Display(Name = "Oda Sayısı")]
        [Range(1, 20, ErrorMessage = "Oda sayısı 1-20 arasında olmalıdır")]
        public int? RoomCount { get; set; }

        [Display(Name = "Banyo Sayısı")]
        [Range(1, 10, ErrorMessage = "Banyo sayısı 1-10 arasında olmalıdır")]
        public int? BathroomCount { get; set; }

        [Display(Name = "Bulunduğu Kat")]
        [Range(-5, 100, ErrorMessage = "Kat numarası -5 ile 100 arasında olmalıdır")]
        public int? FloorNumber { get; set; }

        [Display(Name = "Toplam Kat Sayısı")]
        [Range(1, 100, ErrorMessage = "Toplam kat 1-100 arasında olmalıdır")]
        public int? TotalFloors { get; set; }

        [Display(Name = "Bina Yaşı")]
        [Range(0, 100, ErrorMessage = "Bina yaşı 0-100 arasında olmalıdır")]
        public int? BuildingAge { get; set; }

        [MaxLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir")]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Enlem")]
        [Range(-90, 90, ErrorMessage = "Enlem -90 ile 90 arasında olmalıdır")]
        public double? Latitude { get; set; }

        [Display(Name = "Boylam")]
        [Range(-180, 180, ErrorMessage = "Boylam -180 ile 180 arasında olmalıdır")]
        public double? Longitude { get; set; }

        [Display(Name = "Öne Çıkarılsın")]
        public bool IsFeatured { get; set; } = false;

        [Required(ErrorMessage = "Kategori seçiniz")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Emlak türü seçiniz")]
        [Display(Name = "Emlak Türü")]
        public int PropertyTypeId { get; set; }

        [Required(ErrorMessage = "Şehir seçiniz")]
        [Display(Name = "Şehir")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "İlçe seçiniz")]
        [Display(Name = "İlçe")]
        public int DistrictId { get; set; }

        [Display(Name = "Mahalle")]
        public int? NeighborhoodId { get; set; }

        [Display(Name = "İlan Resimleri")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        // Validation için
        public bool HasValidImages()
        {
            if (Images == null || !Images.Any())
                return true; // Resim zorunlu değilse

            return Images.All(img =>
                img.Length > 0 &&
                img.Length <= 5 * 1024 * 1024 && // 5MB limit
                new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
                    .Contains(Path.GetExtension(img.FileName).ToLower()));
        }
    }
}
