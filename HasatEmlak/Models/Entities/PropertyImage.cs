using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.Entities
{
    public class PropertyImage
    {
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string ImagePath { get; set; }

        [MaxLength(200)]
        public string AltText { get; set; }

        public bool IsMainImage { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int PropertyId { get; set; }

        // Navigation Property
        public virtual Property Property { get; set; }
    }
}

