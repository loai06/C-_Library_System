using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

         [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation property
        public Category? Category { get; set; }
    }
}