using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.Entities
{
    public class Property
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? Area { get; set; } // m2
        public int? RoomCount { get; set; }
        public int? BathroomCount { get; set; }
        public int? FloorNumber { get; set; }
        public int? TotalFloors { get; set; }
        public int? BuildingAge { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public int PropertyTypeId { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int? NeighborhoodId { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; }
        public virtual PropertyType PropertyType { get; set; }
        public virtual City City { get; set; }
        public virtual District District { get; set; }
        public virtual Neighborhood Neighborhood { get; set; }
        public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
    }
}
