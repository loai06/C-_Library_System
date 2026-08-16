using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Services
{
    public interface ICategoryService
    {
        Task<object> GetAllAsync(int pageNumber, int pageSize, string? search, string? sortBy, bool sortDescending);
        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<(bool success, string? errorMessage)> DeleteAsync(int id);
    }
}