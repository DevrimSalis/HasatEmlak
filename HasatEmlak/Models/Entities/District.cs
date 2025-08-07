using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.Entities
{
    public class District
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign Key
        public int CityId { get; set; }

        // Navigation Properties
        public virtual City City { get; set; }
        public virtual ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
