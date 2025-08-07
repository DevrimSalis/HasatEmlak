using HasatEmlak.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Data
{
    public static class SeedData
    {
        public static void ApplySeedData(this ModelBuilder modelBuilder)
        {
            // Kategoriler
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Satılık", Description = "Satılık emlaklar", IconClass = "fas fa-home", IsActive = true, DisplayOrder = 1 },
                new Category { Id = 2, Name = "Kiralık", Description = "Kiralık emlaklar", IconClass = "fas fa-key", IsActive = true, DisplayOrder = 2 }
            );

            // Emlak Türleri
            modelBuilder.Entity<PropertyType>().HasData(
                new PropertyType { Id = 1, Name = "Daire", Description = "Residential apartments", IconClass = "fas fa-building", IsActive = true, DisplayOrder = 1 },
                new PropertyType { Id = 2, Name = "Villa", Description = "Luxury villas", IconClass = "fas fa-house-user", IsActive = true, DisplayOrder = 2 },
                new PropertyType { Id = 3, Name = "Arsa", Description = "Land plots", IconClass = "fas fa-mountain", IsActive = true, DisplayOrder = 3 },
                new PropertyType { Id = 4, Name = "İşyeri", Description = "Commercial properties", IconClass = "fas fa-store", IsActive = true, DisplayOrder = 4 },
                new PropertyType { Id = 5, Name = "Ofis", Description = "Office spaces", IconClass = "fas fa-briefcase", IsActive = true, DisplayOrder = 5 }
            );

            // Şehirler (Türkiye'nin büyük şehirleri)
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "İstanbul", PlateCode = "34", IsActive = true },
                new City { Id = 2, Name = "Ankara", PlateCode = "06", IsActive = true },
                new City { Id = 3, Name = "İzmir", PlateCode = "35", IsActive = true },
                new City { Id = 4, Name = "Antalya", PlateCode = "07", IsActive = true },
                new City { Id = 5, Name = "Bursa", PlateCode = "16", IsActive = true }
            );

            // İlçeler (örnek)
            modelBuilder.Entity<District>().HasData(
                // İstanbul ilçeleri
                new District { Id = 1, Name = "Beşiktaş", CityId = 1, IsActive = true },
                new District { Id = 2, Name = "Şişli", CityId = 1, IsActive = true },
                new District { Id = 3, Name = "Kadıköy", CityId = 1, IsActive = true },

                // Ankara ilçeleri
                new District { Id = 4, Name = "Çankaya", CityId = 2, IsActive = true },
                new District { Id = 5, Name = "Keçiören", CityId = 2, IsActive = true },

                // İzmir ilçeleri
                new District { Id = 6, Name = "Konak", CityId = 3, IsActive = true },
                new District { Id = 7, Name = "Karşıyaka", CityId = 3, IsActive = true }
            );

            // Mahalleler (örnek)
            modelBuilder.Entity<Neighborhood>().HasData(
                new Neighborhood { Id = 1, Name = "Levent", DistrictId = 1, IsActive = true },
                new Neighborhood { Id = 2, Name = "Etiler", DistrictId = 1, IsActive = true },
                new Neighborhood { Id = 3, Name = "Nişantaşı", DistrictId = 2, IsActive = true },
                new Neighborhood { Id = 4, Name = "Moda", DistrictId = 3, IsActive = true },
                new Neighborhood { Id = 5, Name = "Kızılay", DistrictId = 4, IsActive = true }
            );
        }
    }
}
