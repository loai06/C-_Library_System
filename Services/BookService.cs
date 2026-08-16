using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using AutoMapper;

namespace LibraryManagementSystem.Services
{
    using AutoMapper;

    public class BookService : IBookService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BookService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> GetAllAsync(int pageNumber, int pageSize, string? search, int? categoryId, string? sortBy, bool sortDescending)
        {
            var query = _context.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            query = sortBy?.ToLower() switch
            {
                "title" => sortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                "price" => sortDescending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
                _ => query.OrderBy(b => b.Id)
            };

            var totalCount = await query.CountAsync();

            var books = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var bookDtos = _mapper.Map<List<BookResponseDto>>(books);

            return new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = bookDtos
            };
        }

        public async Task<BookResponseDto?> GetByIdAsync(int id)
        {
            var book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
            return book == null ? null : _mapper.Map<BookResponseDto>(book);
        }
        public async Task<(BookResponseDto? result, string? errorMessage)> CreateAsync(BookCreateDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists) return (null, "The specified CategoryId does not exist.");

            var book = _mapper.Map<Book>(dto);
            book.IsDeleted = false;
            book.CreatedAt = DateTime.UtcNow;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var createdBook = await _context.Books.Include(b => b.Category).FirstAsync(b => b.Id == book.Id);
            return (_mapper.Map<BookResponseDto>(createdBook), null);
        }

        public async Task<(bool success, string? errorMessage)> UpdateAsync(int id, BookUpdateDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return (false, "Book not found.");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists) return (false, "The specified CategoryId does not exist.");

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.Price = dto.Price;
            book.Quantity = dto.Quantity;
            book.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            book.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}