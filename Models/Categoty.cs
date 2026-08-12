using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        //Validations
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}