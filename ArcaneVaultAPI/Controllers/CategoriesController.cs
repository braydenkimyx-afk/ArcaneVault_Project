// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVaultAPI.Data;
using ArcaneVaultAPI.DTOs;
using ArcaneVaultAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVaultAPI.Controllers
{
    /// <summary>
    /// Handles category management operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryCode = c.CategoryCode,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return Ok(categories);
        }

        /// <summary>
        /// Gets a specific category by its code.
        /// </summary>
        /// <param name="categoryCode">The category code to look up.</param>
        [HttpGet("{categoryCode}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string categoryCode)
        {
            var category = await _context.Categories.FindAsync(categoryCode);
            if (category == null)
                return NotFound(new ErrorResponseDto { Message = $"Category '{categoryCode}' not found." });

            return Ok(new CategoryDto
            {
                CategoryCode = category.CategoryCode,
                CategoryName = category.CategoryName
            });
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="dto">The category details.</param>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            try
            {
                var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();
                if (callerRole != "Staff")
                    return StatusCode(403, new ErrorResponseDto { Message = "Forbidden: Staff role required." });

                if (string.IsNullOrWhiteSpace(dto.CategoryCode) || string.IsNullOrWhiteSpace(dto.CategoryName))
                    return BadRequest(new ErrorResponseDto { Message = "Category code and name are required." });

                var existing = await _context.Categories.FindAsync(dto.CategoryCode);
                if (existing != null)
                    return BadRequest(new ErrorResponseDto { Message = $"Category code '{dto.CategoryCode}' already exists." });

                var category = new Category
                {
                    CategoryCode = dto.CategoryCode.ToUpper(),
                    CategoryName = dto.CategoryName
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCategory), new { categoryCode = category.CategoryCode }, new CategoryDto
                {
                    CategoryCode = category.CategoryCode,
                    CategoryName = category.CategoryName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="categoryCode">The category code to update.</param>
        /// <param name="dto">Updated category data.</param>
        [HttpPut("{categoryCode}")]
        public async Task<IActionResult> UpdateCategory(string categoryCode, [FromBody] CategoryCreateDto dto)
        {
            try
            {
                var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();
                if (callerRole != "Staff")
                    return StatusCode(403, new ErrorResponseDto { Message = "Forbidden: Staff role required." });

                var category = await _context.Categories.FindAsync(categoryCode);
                if (category == null)
                    return NotFound(new ErrorResponseDto { Message = $"Category '{categoryCode}' not found." });

                category.CategoryName = dto.CategoryName;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Hard-deletes a category. Blocked with 409 if it is referenced by any collection items.
        /// </summary>
        /// <param name="categoryCode">The category code to delete.</param>
        [HttpDelete("{categoryCode}")]
        public async Task<IActionResult> DeleteCategory(string categoryCode)
        {
            try
            {
                var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();
                if (callerRole != "Staff")
                    return StatusCode(403, new ErrorResponseDto { Message = "Forbidden: Staff role required." });

                var category = await _context.Categories.FindAsync(categoryCode);
                if (category == null)
                    return NotFound(new ErrorResponseDto { Message = $"Category '{categoryCode}' not found." });

                var isReferenced = await _context.CollectionItemCategories
                    .AnyAsync(cic => cic.CategoryCode == categoryCode);

                if (isReferenced)
                    return Conflict(new ErrorResponseDto
                    {
                        Message = $"Cannot delete category '{categoryCode}' because it is still referenced by one or more collection items. Remove all item-category associations first."
                    });

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }
    }
}
