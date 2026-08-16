using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            var result = await _categoryService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortDescending);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Categories retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<object>.FailResponse($"Category with id {id} not found.", 404));

            return Ok(ApiResponse<object>.SuccessResponse(category, "Category retrieved successfully."));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostCategory(CategoryCreateDto dto)
        {
            var created = await _categoryService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<object>.SuccessResponse(created, "Category created successfully.", 201));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCategory(int id, CategoryUpdateDto dto)
        {
            var success = await _categoryService.UpdateAsync(id, dto);
            if (!success)
                return NotFound(ApiResponse<object>.FailResponse($"Category with id {id} not found.", 404));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Category updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var (success, errorMessage) = await _categoryService.DeleteAsync(id);
            if (!success)
            {
                var statusCode = errorMessage == "Category not found." ? 404 : 400;
                return StatusCode(statusCode, ApiResponse<object>.FailResponse(errorMessage!, statusCode));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Category deleted successfully."));
        }
    }
}