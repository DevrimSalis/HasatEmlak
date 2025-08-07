using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.Entities
{
    public class Neighborhood
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign Key
        public int DistrictId { get; set; }

        // Navigation Properties
        public virtual District District { get; set; }
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
