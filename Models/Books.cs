using System.ComponentModel.DataAnnotations;

namespace e_BookPvt.Models
{
    public class Books
    {
        [Key]
        public int BookId { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 99999.99)]
        public decimal Price { get; set; } // Book price with validation

        public string ImageUrl { get; set; } // Optional, for storing the book cover image URL
    }
}
