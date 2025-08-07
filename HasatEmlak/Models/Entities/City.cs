using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string PlateCode { get; set; } // Plaka kodu

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<District> Districts { get; set; } = new List<District>();
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
