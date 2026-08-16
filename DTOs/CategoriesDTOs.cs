using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}