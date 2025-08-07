// Data/ApplicationDbContext.cs - BASİTLEŞTİRİLMİŞ VERSİYON
using HasatEmlak.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet'ler
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Property yapılandırması
            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Latitude).HasColumnType("float");
                entity.Property(e => e.Longitude).HasColumnType("float");

                // İlişkiler
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Properties)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PropertyType)
                      .WithMany(pt => pt.Properties)
                      .HasForeignKey(e => e.PropertyTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.City)
                      .WithMany(c => c.Properties)
                      .HasForeignKey(e => e.CityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.District)
                      .WithMany(d => d.Properties)
                      .HasForeignKey(e => e.DistrictId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Neighborhood)
                      .WithMany(n => n.Properties)
                      .HasForeignKey(e => e.NeighborhoodId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // PropertyImage yapılandırması
            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Property)
                      .WithMany(p => p.PropertyImages)
                      .HasForeignKey(e => e.PropertyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // District yapılandırması
            modelBuilder.Entity<District>(entity =>
            {
                entity.HasOne(e => e.City)
                      .WithMany(c => c.Districts)
                      .HasForeignKey(e => e.CityId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Neighborhood yapılandırması
            modelBuilder.Entity<Neighborhood>(entity =>
            {
                entity.HasOne(e => e.District)
                      .WithMany(d => d.Neighborhoods)
                      .HasForeignKey(e => e.DistrictId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ContactRequest yapılandırması
            modelBuilder.Entity<ContactRequest>(entity =>
            {
                entity.HasOne(e => e.Property)
                      .WithMany()
                      .HasForeignKey(e => e.PropertyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
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

            // Şehirler
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "İstanbul", PlateCode = "34", IsActive = true },
                new City { Id = 2, Name = "Ankara", PlateCode = "06", IsActive = true },
                new City { Id = 3, Name = "İzmir", PlateCode = "35", IsActive = true },
                new City { Id = 4, Name = "Antalya", PlateCode = "07", IsActive = true },
                new City { Id = 5, Name = "Bursa", PlateCode = "16", IsActive = true }
            );

            // İlçeler
            modelBuilder.Entity<District>().HasData(
                new District { Id = 1, Name = "Beşiktaş", CityId = 1, IsActive = true },
                new District { Id = 2, Name = "Şişli", CityId = 1, IsActive = true },
                new District { Id = 3, Name = "Kadıköy", CityId = 1, IsActive = true },
                new District { Id = 4, Name = "Çankaya", CityId = 2, IsActive = true },
                new District { Id = 5, Name = "Keçiören", CityId = 2, IsActive = true },
                new District { Id = 6, Name = "Konak", CityId = 3, IsActive = true },
                new District { Id = 7, Name = "Karşıyaka", CityId = 3, IsActive = true }
            );

            // Mahalleler
            modelBuilder.Entity<Neighborhood>().HasData(
                new Neighborhood { Id = 1, Name = "Levent", DistrictId = 1, IsActive = true },
                new Neighborhood { Id = 2, Name = "Etiler", DistrictId = 1, IsActive = true },
                new Neighborhood { Id = 3, Name = "Nişantaşı", DistrictId = 2, IsActive = true },
                new Neighborhood { Id = 4, Name = "Moda", DistrictId = 3, IsActive = true },
                new Neighborhood { Id = 5, Name = "Kızılay", DistrictId = 4, IsActive = true }
            );

            // Örnek İlanlar
            modelBuilder.Entity<Property>().HasData(
                new Property
                {
                    Id = 1,
                    Title = "Çankaya'da Satılık Lüks Daire",
                    Description = "Şehir manzaralı, merkezi konumda, yeni yapı lüks daire. Tüm sosyal olanaklar mevcut.",
                    Price = 2500000,
                    Area = 150,
                    RoomCount = 3,
                    BathroomCount = 2,
                    FloorNumber = 5,
                    TotalFloors = 12,
                    BuildingAge = 2,
                    Address = "Çankaya, Ankara",
                    CategoryId = 1,
                    PropertyTypeId = 1,
                    CityId = 2,
                    DistrictId = 4,
                    NeighborhoodId = 5,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedDate = new DateTime(2024, 12, 25)
                },
                new Property
                {
                    Id = 2,
                    Title = "Beşiktaş'ta Kiralık Ofis",
                    Description = "Metro yakını, modern ofis binasında kiralık ofis. Otopark dahil.",
                    Price = 15000,
                    Area = 85,
                    FloorNumber = 3,
                    TotalFloors = 8,
                    BuildingAge = 5,
                    Address = "Beşiktaş, İstanbul",
                    CategoryId = 2,
                    PropertyTypeId = 5,
                    CityId = 1,
                    DistrictId = 1,
                    NeighborhoodId = 1,
                    IsActive = true,
                    IsFeatured = false,
                    CreatedDate = new DateTime(2024, 12, 30)
                },
                new Property
                {
                    Id = 3,
                    Title = "İzmir Karşıyaka'da Villa",
                    Description = "Deniz manzaralı, bahçeli, lüks villa. Kapalı otopark ve güvenlik mevcut.",
                    Price = 4500000,
                    Area = 300,
                    RoomCount = 5,
                    BathroomCount = 3,
                    TotalFloors = 2,
                    BuildingAge = 3,
                    Address = "Karşıyaka, İzmir",
                    CategoryId = 1,
                    PropertyTypeId = 2,
                    CityId = 3,
                    DistrictId = 7,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedDate = new DateTime(2025, 1, 3)
                }
            );
        }
    }
}