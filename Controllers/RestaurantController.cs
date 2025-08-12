using System.Security.Claims;
using HotByteAPI.Data;
using HotByteAPI.DTOs;
using HotByteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace HotByteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantController(AppDbContext context)
        {
            _context = context;
        }

        // Get branches for this restaurant (based on login)
        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches()
        {
            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (restaurantId == null)
                return Unauthorized();

            var branches = await _context.Branches
                .Where(b => b.RestaurantId == restaurantId)
                .ToListAsync();

            return Ok(branches);
        }

        // CRUD: Get menu items for a branch (restaurant can view only own branch menus)
        [HttpGet("branches/{branchId}/menu")]
        public async Task<IActionResult> GetMenuItems(int branchId)
        {
            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (restaurantId == null)
                return Unauthorized();

            // Validate branch ownership
            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null || branch.RestaurantId != restaurantId)
                return Forbid();

            var menuItems = await _context.MenuItems
                .Where(mi => mi.BranchId == branchId || mi.IsCommon)
                .ToListAsync();

            return Ok(menuItems);
        }

        // CRUD: Add menu item
        [HttpPost("branches/{branchId}/menu")]
        public async Task<IActionResult> AddMenuItem(int branchId, [FromBody] MenuItem menuItem)
        {
            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (restaurantId == null)
                return Unauthorized();

            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null || branch.RestaurantId != restaurantId)
                return Forbid();

            menuItem.BranchId = branchId;
            menuItem.IsCommon = false; // Branch-specific menu item
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItems), new { branchId = branchId }, menuItem);
        }

        // CRUD: Update menu item
        [HttpPut("menu/{menuItemId}")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId, [FromBody] MenuItem updatedItem)
        {
            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (restaurantId == null)
                return Unauthorized();

            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
                return NotFound();

            var branch = await _context.Branches.FindAsync(menuItem.BranchId);
            if (branch == null || branch.RestaurantId != restaurantId)
                return Forbid();

            // Update fields (only editable fields)
            menuItem.Name = updatedItem.Name;
            menuItem.Description = updatedItem.Description;
            menuItem.Category = updatedItem.Category;
            menuItem.Price = updatedItem.Price;
            menuItem.QuantityAvailable = updatedItem.QuantityAvailable;
            menuItem.IsAvailable = updatedItem.IsAvailable;
            menuItem.ImageUrl = updatedItem.ImageUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // CRUD: Delete menu item
        [HttpDelete("menu/{menuItemId}")]
        public async Task<IActionResult> DeleteMenuItem(int menuItemId)
        {
            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (restaurantId == null)
                return Unauthorized();

            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
                return NotFound();

            var branch = await _context.Branches.FindAsync(menuItem.BranchId);
            if (branch == null || branch.RestaurantId != restaurantId)
                return Forbid();

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Get orders for restaurant branches
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (restaurantId == null)
                return Unauthorized();

            var branchIds = await _context.Branches
                .Where(b => b.RestaurantId == restaurantId)
                .Select(b => b.Id)
                .ToListAsync();

            var orders = await _context.Orders
                .Where(o => branchIds.Contains(o.BranchId))
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.User)
                .ToListAsync();

            var response = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                UserEmail = order.User.Email,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        // Update order status and send email notification
        [HttpPut("orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (restaurantId == null)
                return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            var branch = await _context.Branches.FindAsync(order.BranchId);
            if (branch == null || branch.RestaurantId != restaurantId)
                return Forbid();

            order.Status = dto.Status;
            await _context.SaveChangesAsync();

            if (order.User != null && !string.IsNullOrEmpty(order.User.Email))
            {
                await SendOrderStatusUpdateEmail(order.User.Email, order);
            }

            return NoContent();
        }

        private async Task SendOrderStatusUpdateEmail(string toEmail, Order order)
        {
            try
            {
                var fromEmail = "sreenidhichidura@gmail.com"; // Replace with your sender email
                var fromPassword = ""; // Replace with your app password

                var subject = $"Order Update - HotByte #{order.Id}";
                var body = $"Your order #{order.Id} status has been updated to: {order.Status}.\n\nThank you for choosing HotByte!";

                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage(fromEmail, toEmail, subject, body);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
            }
        }
    }

    // DTO for updating order status
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; }
    }
}

