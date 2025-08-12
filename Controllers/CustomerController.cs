using System.Security.Claims;
using HotByteAPI.Data;
using HotByteAPI.DTOs;
using HotByteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotByteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = await _context.Users
                .Include(u => u.ShippingAddress)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound("Profile not found.");

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.PhoneNumber,
                Address = user.ShippingAddress != null ? new
                {
                    user.ShippingAddress.AddressLine,
                    user.ShippingAddress.City,
                    user.ShippingAddress.State,
                    user.ShippingAddress.Pincode,
                    user.ShippingAddress.Country
                } : null
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] CustomerProfileUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = await _context.Users
                .Include(u => u.ShippingAddress)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound("Profile not found.");

            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            if (user.ShippingAddress != null && dto.ShippingAddress != null)
            {
                user.ShippingAddress.AddressLine = dto.ShippingAddress.AddressLine ?? user.ShippingAddress.AddressLine;
                user.ShippingAddress.City = dto.ShippingAddress.City ?? user.ShippingAddress.City;
                user.ShippingAddress.State = dto.ShippingAddress.State ?? user.ShippingAddress.State;
                user.ShippingAddress.Pincode = dto.ShippingAddress.Pincode ?? user.ShippingAddress.Pincode;
                user.ShippingAddress.Country = dto.ShippingAddress.Country ?? user.ShippingAddress.Country;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();

            return Ok(orders.Select(o => new
            {
                o.Id,
                Date = o.CreatedAt,
                o.Status,
                o.PaymentMethod,
                Items = o.OrderItems.Select(i => new
                {
                    i.MenuItem.Name,
                    i.Quantity,
                    i.Price
                })
            }));
        }
    }
}
