using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.Entities
{
    public class PropertyType
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string IconClass { get; set; }

        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;

        // Navigation Property
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
