using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Services
{
    public interface IBookService
    {
        Task<object> GetAllAsync(int pageNumber, int pageSize, string? search, int? categoryId, string? sortBy, bool sortDescending);
        Task<BookResponseDto?> GetByIdAsync(int id);
        Task<(BookResponseDto? result, string? errorMessage)> CreateAsync(BookCreateDto dto);
        Task<(bool success, string? errorMessage)> UpdateAsync(int id, BookUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}