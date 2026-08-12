using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.DTOs;



namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }
[HttpGet]
        public async Task<IActionResult> GetCategories(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            bool sortDescending = false)
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
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = categories
            });
        }

        
        // GET by Id
        [HttpGet("{id}")]
         public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
         {
             var category = await _context.Categories
             .Where(c => c.Id == id)
             .Select(c => new CategoryResponseDto
              {
                   Id = c.Id,
                  Name = c.Name,
                  Description = c.Description,
                  CreatedAt = c.CreatedAt
               })
            .FirstOrDefaultAsync();

             if (category == null)
           {
           return NotFound();
     }

    return category;
}

        // create
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // update
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var hasBooks = await _context.Books.AnyAsync(b => b.CategoryId == id);
            if (hasBooks)
            {
                return BadRequest("Cannot delete a category that still has books.");
            }

            category.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}