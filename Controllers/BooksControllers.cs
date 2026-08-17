using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            int? categoryId = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            var result = await _bookService.GetAllAsync(pageNumber, pageSize, search, categoryId, sortBy, sortDescending);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Books retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound(ApiResponse<object>.FailResponse($"Book with id {id} not found.", 404));

            return Ok(ApiResponse<object>.SuccessResponse(book, "Book retrieved successfully."));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostBook(BookCreateDto dto)
        {
            var (result, error) = await _bookService.CreateAsync(dto);
            if (error != null)
                return BadRequest(ApiResponse<object>.FailResponse(error, 400));

            return StatusCode(201, ApiResponse<object>.SuccessResponse(result, "Book created successfully.", 201));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto dto)
        {
            var (success, error) = await _bookService.UpdateAsync(id, dto);
            if (!success)
            {
                var statusCode = error == "Book not found." ? 404 : 400;
                return StatusCode(statusCode, ApiResponse<object>.FailResponse(error!, statusCode));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Book updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await _bookService.DeleteAsync(id);
            if (!success)
                return NotFound(ApiResponse<object>.FailResponse($"Book with id {id} not found.", 404));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Book deleted successfully."));
        }
    }
}