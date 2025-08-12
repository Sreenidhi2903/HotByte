using HotByteAPI.Data;
using HotByteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HotByteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // View menu items by branch (for all roles)
        [HttpGet("branch/{branchId}")]
        [Authorize]
        public async Task<IActionResult> GetMenuByBranch(int branchId)
        {
            var allItems = await _context.MenuItems
                .Where(m => m.BranchId == branchId || m.BranchId == null)
                .ToListAsync();

            var commonItems = allItems.Where(m => m.BranchId == null).ToList();
            var specialItems = allItems.Where(m => m.BranchId == branchId).ToList();

            var result = new
            {
                CommonItems = commonItems,
                SpecialItems = specialItems
            };

            return Ok(result);
        }

        // Add menu item (Restaurant only)
        [HttpPost]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> AddMenuItem(MenuItem item)
        {
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // Update menu item (Restaurant only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> UpdateMenuItem(int id, MenuItem updatedItem)
        {
            var existingItem = await _context.MenuItems.FindAsync(id);
            if (existingItem == null) return NotFound();

            existingItem.Name = updatedItem.Name;
            existingItem.Description = updatedItem.Description;
            existingItem.Price = updatedItem.Price;
            existingItem.DiscountPercentage = updatedItem.DiscountPercentage;
            existingItem.Category = updatedItem.Category;
            existingItem.ImageUrl = updatedItem.ImageUrl;
            existingItem.IsAvailable = updatedItem.IsAvailable;
            existingItem.QuantityAvailable = updatedItem.QuantityAvailable;
            existingItem.BranchId = updatedItem.BranchId;

            await _context.SaveChangesAsync();
            return Ok(existingItem);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();
            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

//using System.Security.Claims;
//using HotByteAPI.Data;
//using HotByteAPI.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;


//[ApiController]
//[Route("api/[controller]")]
//[Authorize(Roles = "Restaurant")]
//public class MenuController : ControllerBase
//{
//    private readonly AppDbContext _context;

//    public MenuController(AppDbContext context)
//    {
//        _context = context;
//    }

//    // Get all menu items for restaurant's branches
//    [HttpGet]
//    public async Task<IActionResult> GetMenuItems()
//    {
//        var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//        if (restaurantId == null)
//            return Unauthorized();

//        var branchIds = await _context.Branches
//            .Where(b => b.RestaurantId == restaurantId)
//            .Select(b => b.Id)
//            .ToListAsync();

//        var items = await _context.MenuItems
//            .Where(m => branchIds.Contains(m.BranchId.Value))
//            .ToListAsync();

//        return Ok(items);
//    }

//    [HttpPost]
//    public async Task<IActionResult> AddMenuItem([FromBody] MenuItem menuItem)
//    {
//        var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//        if (restaurantId == null)
//            return Unauthorized();

//        // Check branch belongs to restaurant
//        var branch = await _context.Branches.FindAsync(menuItem.BranchId);
//        if (branch == null || branch.RestaurantId != restaurantId)
//            return Forbid();

//        _context.MenuItems.Add(menuItem);
//        await _context.SaveChangesAsync();

//        return CreatedAtAction(nameof(GetMenuItems), new { id = menuItem.Id }, menuItem);
//    }

//    // Similar update, delete actions with validation can be added
//}
