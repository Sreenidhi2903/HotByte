using HotByteAPI.Data;
using HotByteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotByteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Returns string UserId from JWT claims
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ✅ Add item to user's cart
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            string userId = GetUserId();

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.MenuItemId == item.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                item.CartId = cart.Id;
                _context.CartItems.Add(item);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Item added to cart successfully" });
        }

        // ✅ Get cart items
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            string userId = GetUserId();

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return Ok(new List<CartItem>());

            return Ok(cart.Items);
        }

        // ✅ Remove item by ID
        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            string userId = GetUserId();

            var item = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == userId);

            if (item == null)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item removed successfully" });
        }

        // ✅ Clear full cart
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            string userId = GetUserId();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return Ok();

            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cart cleared successfully" });
        }
    }
}

