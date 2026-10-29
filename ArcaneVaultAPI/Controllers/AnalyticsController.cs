// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVaultAPI.Data;
using ArcaneVaultAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVaultAPI.Controllers
{
    /// <summary>
    /// Provides analytics and dashboard data computed server-side via LINQ aggregations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets full dashboard analytics: KPIs, chart data, and low-stock alerts.
        /// Staff sees platform-wide data; Users see only their own items.
        /// </summary>
        /// <param name="userName">Optional username to scope data to a specific user.</param>
        /// <param name="role">The role of the requesting user (Staff sees all).</param>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> GetDashboard([FromQuery] string? userName = null, [FromQuery] string? role = null)
        {
            try
            {
                var query = _context.CollectionItems
                    .Include(ci => ci.CollectionItemCategories)
                    .ThenInclude(cic => cic.Category)
                    .Where(ci => !ci.IsDeleted);

                // Scope by user unless staff. Fail closed: a non-staff caller with no
                // username (e.g. unauthenticated) gets nothing, never the unscoped set.
                if (role != "Staff")
                {
                    if (string.IsNullOrWhiteSpace(userName))
                        return Ok(new DashboardDto());
                    query = query.Where(ci => ci.UserName == userName);
                }

                var items = await query.ToListAsync();

                var totalItems = items.Count;
                var totalQuantity = items.Sum(i => i.CurrentQuantity);
                var totalStarting = items.Sum(i => i.StartingQuantity);
                var categoriesUsed = items
                    .SelectMany(i => i.CollectionItemCategories)
                    .Select(cic => cic.CategoryCode)
                    .Distinct()
                    .Count();
                // Per-item average depletion: mean of each item's (start−current)/start
                var depletionRate = items.Count > 0
                    ? Math.Round(items
                        .Where(i => i.StartingQuantity > 0)
                        .Select(i => ((double)(i.StartingQuantity - i.CurrentQuantity) / i.StartingQuantity) * 100)
                        .DefaultIfEmpty(0)
                        .Average(), 1)
                    : 0;

                // Category distribution (doughnut chart)
                var categoryDistribution = items
                    .SelectMany(i => i.CollectionItemCategories)
                    .GroupBy(cic => new { cic.CategoryCode, CategoryName = cic.Category?.CategoryName ?? "" })
                    .Select(g => new CategoryDistributionDto
                    {
                        CategoryCode = g.Key.CategoryCode,
                        CategoryName = g.Key.CategoryName,
                        ItemCount = g.Count()
                    })
                    .OrderByDescending(c => c.ItemCount)
                    .ToList();

                // Top 5 categories (bar chart)
                var topCategories = categoryDistribution
                    .Take(5)
                    .Select(c => new TopCategoryDto
                    {
                        CategoryName = c.CategoryName,
                        ItemCount = c.ItemCount
                    })
                    .ToList();

                // Quantity depletion (line chart)
                var quantityDepletion = items
                    .OrderByDescending(i => i.StartingQuantity - i.CurrentQuantity)
                    .Take(10)
                    .Select(i => new QuantityDepletionDto
                    {
                        ItemName = i.ItemName,
                        StartingQuantity = i.StartingQuantity,
                        CurrentQuantity = i.CurrentQuantity
                    })
                    .ToList();

                // Low stock alerts (CurrentQuantity <= 20% of StartingQuantity)
                var lowStockItems = items
                    .Where(i => i.StartingQuantity > 0 && (double)i.CurrentQuantity / i.StartingQuantity <= 0.201)
                    .Select(i => new LowStockItemDto
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        StartingQuantity = i.StartingQuantity,
                        CurrentQuantity = i.CurrentQuantity,
                        PercentRemaining = Math.Round(((double)i.CurrentQuantity / i.StartingQuantity) * 100, 1),
                        HasImage = i.ImageData != null && i.ImageData.Length > 0
                    })
                    .OrderBy(i => i.PercentRemaining)
                    .ToList();

                return Ok(new DashboardDto
                {
                    TotalItems = totalItems,
                    TotalQuantity = totalQuantity,
                    CategoriesUsed = categoriesUsed,
                    DepletionRate = depletionRate,
                    CategoryDistribution = categoryDistribution,
                    TopCategories = topCategories,
                    QuantityDepletion = quantityDepletion,
                    LowStockItems = lowStockItems
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Gets low-stock items where CurrentQuantity is 20% or less of StartingQuantity.
        /// </summary>
        /// <param name="userName">Optional username to scope results.</param>
        /// <param name="role">The role of the requesting user.</param>
        [HttpGet("lowstock")]
        public async Task<ActionResult<IEnumerable<LowStockItemDto>>> GetLowStockItems([FromQuery] string? userName = null, [FromQuery] string? role = null)
        {
            try
            {
                var query = _context.CollectionItems.Where(ci => !ci.IsDeleted && ci.StartingQuantity > 0);

                // Fail closed: a non-staff caller with no username gets nothing.
                if (role != "Staff")
                {
                    if (string.IsNullOrWhiteSpace(userName))
                        return Ok(new List<LowStockItemDto>());
                    query = query.Where(ci => ci.UserName == userName);
                }

                var items = await query.ToListAsync();

                var lowStock = items
                    .Where(i => (double)i.CurrentQuantity / i.StartingQuantity <= 0.201)
                    .Select(i => new LowStockItemDto
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        StartingQuantity = i.StartingQuantity,
                        CurrentQuantity = i.CurrentQuantity,
                        PercentRemaining = Math.Round(((double)i.CurrentQuantity / i.StartingQuantity) * 100, 1),
                        HasImage = i.ImageData != null && i.ImageData.Length > 0
                    })
                    .OrderBy(i => i.PercentRemaining)
                    .ToList();

                return Ok(lowStock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Exports the user's collection as CSV data.
        /// </summary>
        /// <param name="userName">Username whose collection to export.</param>
        /// <param name="role">The role of the requesting user (Staff can export all).</param>
        [HttpGet("export")]
        public async Task<IActionResult> ExportCsv([FromQuery] string? userName = null, [FromQuery] string? role = null)
        {
            try
            {
                var query = _context.CollectionItems
                    .Include(ci => ci.CollectionItemCategories)
                    .ThenInclude(cic => cic.Category)
                    .Where(ci => !ci.IsDeleted);

                // Fail closed: a non-staff caller with no username gets nothing.
                if (role != "Staff")
                {
                    if (string.IsNullOrWhiteSpace(userName))
                        query = query.Where(ci => false);
                    else
                        query = query.Where(ci => ci.UserName == userName);
                }

                var items = await query.ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("ItemId,ItemName,StartingQuantity,CurrentQuantity,UserName,Categories");

                foreach (var item in items)
                {
                    var categories = string.Join("; ", item.CollectionItemCategories
                        .Select(cic => cic.Category?.CategoryName ?? cic.CategoryCode));
                    csv.AppendLine($"{item.ItemId},\"{item.ItemName}\",{item.StartingQuantity},{item.CurrentQuantity},\"{item.UserName}\",\"{categories}\"");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", "collection_export.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Gets type-completion data: which categories the user owns at least one (non-deleted) item in.
        /// Staff sees platform-wide completion; Users see only their own.
        /// </summary>
        [HttpGet("completion")]
        public async Task<ActionResult<CompletionDto>> GetCompletion([FromQuery] string? userName = null, [FromQuery] string? role = null)
        {
            try
            {
                // All platform categories
                var allCategories = await _context.Categories
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();

                // Build scoped item query — explicitly exclude soft-deleted items
                var itemQuery = _context.CollectionItems
                    .Include(ci => ci.CollectionItemCategories)
                    .Where(ci => !ci.IsDeleted);

                // Scope: Staff sees platform-wide, User sees own only. Fail closed.
                if (role != "Staff")
                {
                    if (string.IsNullOrWhiteSpace(userName))
                        return Ok(new CompletionDto { TotalCategories = allCategories.Count });
                    itemQuery = itemQuery.Where(ci => ci.UserName == userName);
                }

                // Distinct category codes the user owns items in
                var ownedCodes = await itemQuery
                    .SelectMany(ci => ci.CollectionItemCategories)
                    .Select(cic => cic.CategoryCode)
                    .Distinct()
                    .ToListAsync();

                var ownedSet = new HashSet<string>(ownedCodes, StringComparer.OrdinalIgnoreCase);

                var categoryList = allCategories.Select(c => new CompletionCategoryDto
                {
                    CategoryCode = c.CategoryCode,
                    CategoryName = c.CategoryName,
                    Owned = ownedSet.Contains(c.CategoryCode)
                }).ToList();

                var total = allCategories.Count;
                var owned = ownedSet.Count;
                var pct = total > 0 ? Math.Round((double)owned / total * 100, 1) : 0;

                return Ok(new CompletionDto
                {
                    TotalCategories = total,
                    Categories = categoryList,
                    CompletionPercentage = pct
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }
    }
}
