// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVaultAPI.Data;
using ArcaneVaultAPI.DTOs;
using ArcaneVaultAPI.Models;
using ArcaneVaultAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVaultAPI.Controllers
{
    /// <summary>
    /// Handles collection item CRUD and search operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CollectionItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CollectionItemsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all non-deleted collection items. Scoped by X-User-Name unless Staff.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollectionItemDto>>> GetItems()
        {
            var callerName = Request.Headers["X-User-Name"].FirstOrDefault();
            var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();

            var query = _context.CollectionItems
                .Include(ci => ci.CollectionItemCategories)
                .ThenInclude(cic => cic.Category)
                .Where(ci => !ci.IsDeleted);

            if (callerRole != "Staff" && !string.IsNullOrWhiteSpace(callerName))
                query = query.Where(ci => ci.UserName == callerName);

            var items = await query.Select(ci => new CollectionItemDto
            {
                ItemId = ci.ItemId,
                ItemName = ci.ItemName,
                IsDeleted = ci.IsDeleted,
                StartingQuantity = ci.StartingQuantity,
                CurrentQuantity = ci.CurrentQuantity,
                UserName = ci.UserName,
                Categories = ci.CollectionItemCategories.Select(cic => new CategoryDto
                {
                    CategoryCode = cic.CategoryCode,
                    CategoryName = cic.Category != null ? cic.Category.CategoryName : ""
                }).ToList(),
                HasImage = ci.ImageData != null && ci.ImageData.Length > 0,
                Price = ci.Price
            }).ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// Gets the most recently catalogued items (unscoped, available to all).
        /// </summary>
        /// <param name="take">Number of items to retrieve (limit clamped 1-50).</param>
        [HttpGet("recent")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CollectionItemDto>>> GetRecentItems([FromQuery] int take = 10)
        {
            int limit = Math.Clamp(take, 1, 50);

            var items = await _context.CollectionItems
                .Include(ci => ci.CollectionItemCategories)
                .ThenInclude(cic => cic.Category)
                .Where(ci => !ci.IsDeleted) // FIXED: Filter ONLY on IsDeleted, NOT user-scoped.
                .OrderByDescending(ci => ci.ItemId) // FIXED: Uses identity descending as recent ordering proxy.
                .Take(limit)
                .Select(ci => new CollectionItemDto
                {
                    ItemId = ci.ItemId,
                    ItemName = ci.ItemName,
                    IsDeleted = ci.IsDeleted,
                    StartingQuantity = ci.StartingQuantity,
                    CurrentQuantity = ci.CurrentQuantity,
                    UserName = ci.UserName,
                    Categories = ci.CollectionItemCategories.Select(cic => new CategoryDto
                    {
                        CategoryCode = cic.CategoryCode,
                        CategoryName = cic.Category != null ? cic.Category.CategoryName : ""
                    }).ToList(),
                    HasImage = ci.ImageData != null && ci.ImageData.Length > 0,
                    Price = ci.Price
                }).ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// Gets a paginated, filtered, and sorted list of collection items across all users.
        /// </summary>
        [HttpGet("browse")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<ActionResult<BrowseResponseDto>> GetBrowseItems(
            [FromQuery] string search = "",
            [FromQuery] string category = "",
            [FromQuery] string sort = "newest",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 24,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            try
            {
                var query = _context.CollectionItems
                    .Include(ci => ci.CollectionItemCategories)
                    .ThenInclude(cic => cic.Category)
                    .Where(ci => !ci.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.ToLower();
                    query = query.Where(ci => 
                        ci.ItemName.ToLower().Contains(term) ||
                        ci.UserName.ToLower().Contains(term) ||
                        ci.CollectionItemCategories.Any(cic => 
                            cic.CategoryCode.ToLower().Contains(term) || 
                            (cic.Category != null && cic.Category.CategoryName.ToLower().Contains(term))
                        )
                    );
                }

                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(ci => ci.CollectionItemCategories.Any(cic => cic.CategoryCode == category));
                }

                if (minPrice.HasValue)
                    query = query.Where(ci => ci.Price != null && ci.Price >= minPrice.Value);
                if (maxPrice.HasValue)
                    query = query.Where(ci => ci.Price != null && ci.Price <= maxPrice.Value);

                int totalCount = await query.CountAsync();
                int limit = Math.Clamp(pageSize, 1, 48);
                int totalPages = (int)Math.Ceiling(totalCount / (double)limit);
                if (totalPages == 0) totalPages = 1;
                
                // Clamp page to valid range
                page = Math.Clamp(page, 1, totalPages);

                switch (sort?.ToLower())
                {
                    case "name":
                        query = query.OrderBy(ci => ci.ItemName).ThenByDescending(ci => ci.ItemId);
                        break;
                    case "quantity":
                        query = query.OrderByDescending(ci => ci.CurrentQuantity).ThenByDescending(ci => ci.ItemId);
                        break;
                    case "price_asc":
                        query = query.OrderBy(ci => ci.Price == null).ThenBy(ci => ci.Price).ThenByDescending(ci => ci.ItemId);
                        break;
                    case "price_desc":
                        query = query.OrderBy(ci => ci.Price == null).ThenByDescending(ci => ci.Price).ThenByDescending(ci => ci.ItemId);
                        break;
                    case "newest":
                    default:
                        query = query.OrderByDescending(ci => ci.ItemId);
                        break;
                }

                var items = await query
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .Select(ci => new BrowseItemDto
                    {
                        ItemId = ci.ItemId,
                        ItemName = ci.ItemName,
                        StartingQuantity = ci.StartingQuantity,
                        CurrentQuantity = ci.CurrentQuantity,
                        UserName = ci.UserName,
                        Categories = ci.CollectionItemCategories.Select(cic => new CategoryDto
                        {
                            CategoryCode = cic.CategoryCode,
                            CategoryName = cic.Category != null ? cic.Category.CategoryName : ""
                        }).ToList(),
                        HasImage = ci.ImageData != null && ci.ImageData.Length > 0,
                        Price = ci.Price
                    }).ToListAsync();

                return Ok(new BrowseResponseDto
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = limit,
                    TotalPages = totalPages,
                    Items = items
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "Error fetching browse items.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Gets a specific collection item for public browsing (unscoped).
        /// </summary>
        [HttpGet("browse/{id:int}")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<ActionResult<BrowseItemDto>> GetBrowseItem(int id)
        {
            var item = await _context.CollectionItems
                .Include(ci => ci.CollectionItemCategories)
                .ThenInclude(cic => cic.Category)
                .FirstOrDefaultAsync(ci => ci.ItemId == id && !ci.IsDeleted);

            if (item == null)
                return NotFound(new ErrorResponseDto { Message = $"Collection item with ID {id} not found." });

            return Ok(new BrowseItemDto
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                StartingQuantity = item.StartingQuantity,
                CurrentQuantity = item.CurrentQuantity,
                UserName = item.UserName,
                Categories = item.CollectionItemCategories.Select(cic => new CategoryDto
                {
                    CategoryCode = cic.CategoryCode,
                    CategoryName = cic.Category?.CategoryName ?? ""
                }).ToList(),
                HasImage = item.ImageData != null && item.ImageData.Length > 0,
                Price = item.Price
            });
        }

        /// <summary>
        /// Gets a specific collection item by its ID.
        /// </summary>
        /// <param name="id">The item ID.</param>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CollectionItemDto>> GetItem(int id)
        {
            var callerName = Request.Headers["X-User-Name"].FirstOrDefault();
            var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();

            var item = await _context.CollectionItems
                .Include(ci => ci.CollectionItemCategories)
                .ThenInclude(cic => cic.Category)
                .FirstOrDefaultAsync(ci => ci.ItemId == id);

            if (item == null || item.IsDeleted)
                return NotFound(new ErrorResponseDto { Message = $"Collection item with ID {id} not found." });

            if (callerRole != "Staff" && item.UserName != callerName)
                return StatusCode(403, new ErrorResponseDto { Message = "Forbidden: You do not own this item." });

            return Ok(new CollectionItemDto
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                IsDeleted = item.IsDeleted,
                StartingQuantity = item.StartingQuantity,
                CurrentQuantity = item.CurrentQuantity,
                UserName = item.UserName,
                Categories = item.CollectionItemCategories.Select(cic => new CategoryDto
                {
                    CategoryCode = cic.CategoryCode,
                    CategoryName = cic.Category?.CategoryName ?? ""
                }).ToList(),
                HasImage = item.ImageData != null && item.ImageData.Length > 0,
                Price = item.Price
            });
        }

        /// <summary>
        /// Retrieves the raw image bytes for a specified collection item.
        /// </summary>
        /// <param name="id">The item ID.</param>
        [HttpGet("{id:int}/image")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetItemImage(int id)
        {
            var item = await _context.CollectionItems
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.ItemId == id);

            if (item == null || item.IsDeleted || item.ImageData == null || item.ImageContentType == null)
            {
                return NotFound();
            }

            Response.Headers.Append("Cache-Control", "public, max-age=3600");
            return File(item.ImageData, item.ImageContentType);
        }

        /// <summary>
        /// Creates a new collection item with associated categories.
        /// </summary>
        /// <param name="dto">The item details including category codes.</param>
        [HttpPost]
        public async Task<ActionResult<CollectionItemDto>> CreateItem([FromBody] CollectionItemCreateDto dto)
        {
            try
            {
                var callerName = Request.Headers["X-User-Name"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(callerName))
                    return StatusCode(403, new ErrorResponseDto { Message = "Forbidden: Unknown user." });

                // Force the user name to the caller name, ignoring what the client sent
                dto.UserName = callerName;

                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(dto.ItemName))
                    errors.Add("Item name is required.");
                if (string.IsNullOrWhiteSpace(dto.UserName))
                    errors.Add("Username is required.");
                if (dto.CurrentQuantity > dto.StartingQuantity)
                    errors.Add("Current quantity cannot exceed starting quantity.");
                if (dto.StartingQuantity < 0)
                    errors.Add("Starting quantity must be non-negative.");
                if (dto.Price.HasValue && dto.Price < 0)
                    errors.Add("Price cannot be negative.");

                if (errors.Count > 0)
                    return BadRequest(new ErrorResponseDto { Message = "Validation failed.", Errors = errors });

                // Verify user exists
                var user = await _context.ArcaneVaultUsers.FindAsync(dto.UserName);
                if (user == null || user.IsDeleted)
                    return BadRequest(new ErrorResponseDto { Message = $"User '{dto.UserName}' not found." });

                byte[]? imageData = null;
                string? imageContentType = null;

                if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
                {
                    try
                    {
                        imageData = Convert.FromBase64String(dto.ImageBase64);
                        imageContentType = dto.ImageContentType;
                        
                        var (isValid, errorMessage) = ImageValidator.ValidateImage(imageData, imageContentType);
                        if (!isValid)
                            return BadRequest(new ErrorResponseDto { Message = errorMessage });
                    }
                    catch (FormatException)
                    {
                        return BadRequest(new ErrorResponseDto { Message = "Invalid base64 image data." });
                    }
                }

                var item = new CollectionItem
                {
                    ItemName = dto.ItemName,
                    IsDeleted = false,
                    StartingQuantity = dto.StartingQuantity,
                    CurrentQuantity = dto.CurrentQuantity,
                    UserName = dto.UserName,
                    ImageData = imageData,
                    ImageContentType = imageContentType,
                    Price = dto.Price
                };

                _context.CollectionItems.Add(item);
                await _context.SaveChangesAsync();

                // Add category associations
                if (dto.CategoryCodes.Count > 0)
                {
                    foreach (var code in dto.CategoryCodes)
                    {
                        var cat = await _context.Categories.FindAsync(code);
                        if (cat != null)
                        {
                            _context.CollectionItemCategories.Add(new CollectionItemCategory
                            {
                                ItemId = item.ItemId,
                                CategoryCode = code
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Reload with categories
                var created = await _context.CollectionItems
                    .Include(ci => ci.CollectionItemCategories)
                    .ThenInclude(cic => cic.Category)
                    .FirstAsync(ci => ci.ItemId == item.ItemId);

                var result = new CollectionItemDto
                {
                    ItemId = created.ItemId,
                    ItemName = created.ItemName,
                    IsDeleted = created.IsDeleted,
                    StartingQuantity = created.StartingQuantity,
                    CurrentQuantity = created.CurrentQuantity,
                    UserName = created.UserName,
                    Categories = created.CollectionItemCategories.Select(cic => new CategoryDto
                    {
                        CategoryCode = cic.CategoryCode,
                        CategoryName = cic.Category?.CategoryName ?? ""
                    }).ToList(),
                    HasImage = created.ImageData != null && created.ImageData.Length > 0,
                    Price = created.Price
                };

                return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Updates an existing collection item and its category associations.
        /// </summary>
        /// <param name="id">The item ID to update.</param>
        /// <param name="dto">Updated item data.</param>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] CollectionItemUpdateDto dto)
        {
            try
            {
                var callerName = Request.Headers["X-User-Name"].FirstOrDefault();
                var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();

                var item = await _context.CollectionItems
                    .Include(ci => ci.CollectionItemCategories)
                    .FirstOrDefaultAsync(ci => ci.ItemId == id);

                if (item == null || item.IsDeleted)
                    return NotFound(new ErrorResponseDto { Message = $"Collection item with ID {id} not found." });

                if (item.UserName != callerName)
                    return StatusCode(403, new ErrorResponseDto { Message = "Forbidden: You do not own this item." });

                if (dto.CurrentQuantity > dto.StartingQuantity)
                    return BadRequest(new ErrorResponseDto { Message = "Current quantity cannot exceed starting quantity." });

                if (dto.Price.HasValue && dto.Price < 0)
                    return BadRequest(new ErrorResponseDto { Message = "Price cannot be negative." });

                if (dto.RemoveImage)
                {
                    item.ImageData = null;
                    item.ImageContentType = null;
                }
                else if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
                {
                    try
                    {
                        var imageData = Convert.FromBase64String(dto.ImageBase64);
                        var imageContentType = dto.ImageContentType;
                        
                        var (isValid, errorMessage) = ImageValidator.ValidateImage(imageData, imageContentType);
                        if (!isValid)
                            return BadRequest(new ErrorResponseDto { Message = errorMessage });
                            
                        item.ImageData = imageData;
                        item.ImageContentType = imageContentType;
                    }
                    catch (FormatException)
                    {
                        return BadRequest(new ErrorResponseDto { Message = "Invalid base64 image data." });
                    }
                }

                item.ItemName = dto.ItemName;
                item.StartingQuantity = dto.StartingQuantity;
                item.CurrentQuantity = dto.CurrentQuantity;
                item.Price = dto.Price;

                // Update category associations — remove old, add new
                _context.CollectionItemCategories.RemoveRange(item.CollectionItemCategories);

                foreach (var code in dto.CategoryCodes)
                {
                    var cat = await _context.Categories.FindAsync(code);
                    if (cat != null)
                    {
                        _context.CollectionItemCategories.Add(new CollectionItemCategory
                        {
                            ItemId = item.ItemId,
                            CategoryCode = code
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Soft-deletes a collection item by setting IsDeleted to true.
        /// </summary>
        /// <param name="id">The item ID to delete.</param>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var callerName = Request.Headers["X-User-Name"].FirstOrDefault();
                var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();

                var item = await _context.CollectionItems.FindAsync(id);
                if (item == null || item.IsDeleted)
                    return NotFound(new ErrorResponseDto { Message = $"Collection item with ID {id} not found." });

                if (callerRole != "Staff" && item.UserName != callerName)
                    return StatusCode(403, new ErrorResponseDto { Message = "Forbidden: You do not own this item." });

                item.IsDeleted = true;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Searches collection items across ALL fields: ItemId, ItemName, StartingQuantity, CurrentQuantity,
        /// UserName, and associated CategoryName/CategoryCode. Case-insensitive partial match.
        /// </summary>
        /// <param name="q">The search query string.</param>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CollectionItemDto>>> SearchItems([FromQuery] string q = "")
        {
            try
            {
                var callerName = Request.Headers["X-User-Name"].FirstOrDefault();
                var callerRole = Request.Headers["X-User-Role"].FirstOrDefault();

                var query = _context.CollectionItems
                    .Include(ci => ci.CollectionItemCategories)
                    .ThenInclude(cic => cic.Category)
                    .Where(ci => !ci.IsDeleted);

                if (callerRole != "Staff" && !string.IsNullOrWhiteSpace(callerName))
                    query = query.Where(ci => ci.UserName == callerName);

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var searchTerm = q.ToLower();
                    bool isNumeric = int.TryParse(q, out int numericTerm);
                    
                    // (A2) Bug fix: When the search term is numeric, we match numeric columns (ItemId, StartingQuantity, CurrentQuantity) 
                    // by exact equality (==) to prevent SQL translation errors from .ToString().Contains().
                    // We still run the string Contains on text fields to allow searching usernames or names that happen to be numbers.
                    query = query.Where(ci =>
                        (isNumeric && (ci.ItemId == numericTerm || ci.StartingQuantity == numericTerm || ci.CurrentQuantity == numericTerm)) ||
                        ci.ItemName.ToLower().Contains(searchTerm) ||
                        ci.UserName.ToLower().Contains(searchTerm) ||
                        ci.CollectionItemCategories.Any(cic =>
                            cic.CategoryCode.ToLower().Contains(searchTerm) ||
                            (cic.Category != null && cic.Category.CategoryName.ToLower().Contains(searchTerm))
                        )
                    );
                }

                var items = await query.Select(ci => new CollectionItemDto
                {
                    ItemId = ci.ItemId,
                    ItemName = ci.ItemName,
                    IsDeleted = ci.IsDeleted,
                    StartingQuantity = ci.StartingQuantity,
                    CurrentQuantity = ci.CurrentQuantity,
                    UserName = ci.UserName,
                    Categories = ci.CollectionItemCategories.Select(cic => new CategoryDto
                    {
                        CategoryCode = cic.CategoryCode,
                        CategoryName = cic.Category != null ? cic.Category.CategoryName : ""
                    }).ToList(),
                    HasImage = ci.ImageData != null && ci.ImageData.Length > 0,
                    Price = ci.Price
                }).ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred during search.", Errors = new List<string> { ex.Message } });
            }
        }
    }
}
