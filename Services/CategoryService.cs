using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using AutoMapper;

namespace LibraryManagementSystem.Services
{
   public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

       public async Task<object> GetAllAsync(int pageNumber, int pageSize, string? search, string? sortBy, bool sortDescending)
{
    var query = _context.Categories.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(c => c.Name.Contains(search));
    }

    query = sortBy?.ToLower() switch
    {
        "name" => sortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
        _ => query.OrderBy(c => c.Id)
    };

    var totalCount = await query.CountAsync();

    var categories = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var categoryDtos = _mapper.Map<List<CategoryResponseDto>>(categories);

    return new
    {
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize,
        Items = categoryDtos
    };
}

       public async Task<CategoryResponseDto?> GetByIdAsync(int id)
{
    var category = await _context.Categories.FindAsync(id);
    return category == null ? null : _mapper.Map<CategoryResponseDto>(category);
}

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
{
    var category = _mapper.Map<Category>(dto);
    category.IsDeleted = false;
    category.CreatedAt = DateTime.UtcNow;

    _context.Categories.Add(category);
    await _context.SaveChangesAsync();

    return _mapper.Map<CategoryResponseDto>(category);
}

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool success, string? errorMessage)> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return (false, "Category not found.");

            var hasBooks = await _context.Books.AnyAsync(b => b.CategoryId == id);
            if (hasBooks) return (false, "Cannot delete a category that still has books.");

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}