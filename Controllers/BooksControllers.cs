using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetBooks(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            int? categoryId = null,
            string? sortBy = null,
            bool sortDescending = false)
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
                .Select(b => new BookResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price,
                    Quantity = b.Quantity,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category!.Name
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = books
            }
        );
        }

      

        // GET by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetBook(int id)
     {
          var book = await _context.Books
        .Include(b => b.Category)
        .Where(b => b.Id == id)
        .Select(b => new BookResponseDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Price = b.Price,
                Quantity = b.Quantity,
                CategoryId = b.CategoryId,
                CategoryName = b.Category!.Name
          })
        .FirstOrDefaultAsync(); 

    if (book == null)
    {
        return NotFound();
    }

    return book;
}

        // create
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == book.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("The specified CategoryId does not exist.");
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // update
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == book.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("The specified CategoryId does not exist.");
            }

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // delete (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}