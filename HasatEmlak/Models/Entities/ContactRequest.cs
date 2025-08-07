using System.ComponentModel.DataAnnotations;

namespace HasatEmlak.Models.Entities
{
    public class ContactRequest
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key (Opsiyonel - hangi ilan için mesaj)
        public int? PropertyId { get; set; }

        // Navigation Property
        public virtual Property Property { get; set; }
    }
}
