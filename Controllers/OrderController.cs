//using System.Net;
//using System.Net.Mail;
//using System.Security.Claims;
//using HotByteAPI.Data;
//using HotByteAPI.DTOs;
//using HotByteAPI.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace HotByteAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize(Roles = "User,Restaurant")]  // allow both roles
//    public class OrderController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public OrderController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // User places order
//        [HttpPost("place")]
//        [Authorize(Roles = "User")]
//        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (userId == null)
//                return Unauthorized("Invalid user.");

//            if (dto == null || dto.Items == null || !dto.Items.Any())
//                return BadRequest("Order items cannot be empty.");

//            using var transaction = await _context.Database.BeginTransactionAsync();
//            try
//            {
//                var order = new Order
//                {
//                    UserId = userId,
//                    BranchId = dto.BranchId,
//                    PaymentMethod = dto.PaymentMethod,
//                    Status = "Pending",
//                    TotalAmount = 0,
//                    OrderItems = new List<OrderItem>()
//                };

//                foreach (var item in dto.Items)
//                {
//                    var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
//                    if (menuItem == null)
//                        return BadRequest($"Invalid menu item ID: {item.MenuItemId}");

//                    var discount = menuItem.DiscountPercentage ?? 0;
//                    var priceAfterDiscount = menuItem.Price - (menuItem.Price * discount / 100);

//                    var orderItem = new OrderItem
//                    {
//                        MenuItemId = menuItem.Id,
//                        Quantity = item.Quantity,
//                        Price = priceAfterDiscount * item.Quantity
//                    };

//                    order.TotalAmount += orderItem.Price;
//                    order.OrderItems.Add(orderItem);
//                }

//                _context.Orders.Add(order);
//                await _context.SaveChangesAsync(); // Save to get Order.Id

//                var shippingAddress = new ShippingAddress
//                {
//                    UserId = userId,
//                    AddressLine = dto.ShippingAddress.AddressLine,
//                    City = dto.ShippingAddress.City,
//                    Pincode = dto.ShippingAddress.Pincode,
//                    State = dto.ShippingAddress.State,
//                    Country = dto.ShippingAddress.Country
//                };

//                _context.ShippingAddresses.Add(shippingAddress);
//                await _context.SaveChangesAsync();

//                await transaction.CommitAsync();

//                var response = new OrderResponseDto
//                {
//                    Id = order.Id,
//                    CreatedAt = order.CreatedAt,
//                    Status = order.Status,
//                    TotalAmount = order.TotalAmount,
//                    Items = order.OrderItems.Select(oi => new OrderItemResponseDto
//                    {
//                        MenuItemId = oi.MenuItemId,
//                        Quantity = oi.Quantity,
//                        Price = oi.Price
//                    }).ToList()
//                };

//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                await transaction.RollbackAsync();
//                return StatusCode(500, new { error = "Failed to place order.", details = ex.ToString() });
//            }
//        }

//        // User views own orders
//        [HttpGet("my")]
//        [Authorize(Roles = "User")]
//        public async Task<IActionResult> GetMyOrders()
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (userId == null)
//                return Unauthorized();

//            var orders = await _context.Orders
//                .Where(o => o.UserId == userId)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.MenuItem)
//                .ToListAsync();

//            var response = orders.Select(order => new OrderResponseDto
//            {
//                Id = order.Id,
//                CreatedAt = order.CreatedAt,
//                Status = order.Status,
//                TotalAmount = order.TotalAmount,
//                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
//                {
//                    MenuItemId = oi.MenuItemId,
//                    MenuItemName = oi.MenuItem?.Name ?? "Unknown",
//                    Quantity = oi.Quantity,
//                    Price = oi.Price
//                }).ToList()
//            }).ToList();

//            return Ok(response);
//        }

//        // Restaurant: get orders for their branch(es)
//        [HttpGet("restaurant")]
//        [Authorize(Roles = "Restaurant")]
//        public async Task<IActionResult> GetOrdersForRestaurant()
//        {
//            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (restaurantId == null)
//                return Unauthorized();

//            // Assumes Branch has RestaurantId field linking it to the restaurant user
//            var branches = await _context.Branches
//                .Where(b => b.RestaurantId == restaurantId)
//                .Select(b => b.Id)
//                .ToListAsync();

//            var orders = await _context.Orders
//                .Where(o => branches.Contains(o.BranchId))
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.MenuItem)
//                .Include(o => o.User)
//                .ToListAsync();

//            var response = orders.Select(order => new OrderResponseDto
//            {
//                Id = order.Id,
//                CreatedAt = order.CreatedAt,
//                Status = order.Status,
//                TotalAmount = order.TotalAmount,
//                UserEmail = order.User.Email,
//                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
//                {
//                    MenuItemId = oi.MenuItemId,
//                    MenuItemName = oi.MenuItem?.Name ?? "Unknown",
//                    Quantity = oi.Quantity,
//                    Price = oi.Price
//                }).ToList()
//            }).ToList();

//            return Ok(response);
//        }

//        // Restaurant: update order status
//        [HttpPut("{orderId}/status")]
//        [Authorize(Roles = "Restaurant")]
//        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
//        {
//            var restaurantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (restaurantId == null)
//                return Unauthorized();

//            var order = await _context.Orders
//                .Include(o => o.User)
//                .FirstOrDefaultAsync(o => o.Id == orderId);

//            if (order == null)
//                return NotFound();

//            // Check if the order belongs to restaurant's branch
//            var branch = await _context.Branches.FindAsync(order.BranchId);
//            if (branch == null || branch.RestaurantId != restaurantId)
//                return Forbid();

//            order.Status = dto.Status;
//            await _context.SaveChangesAsync();

//            // Send email notification about status update
//            if (order.User != null && !string.IsNullOrEmpty(order.User.Email))
//            {
//                await SendOrderStatusUpdateEmail(order.User.Email, order);
//            }

//            return NoContent();
//        }

//        private async Task SendOrderConfirmationEmail(string toEmail, Order order)
//        {
//            try
//            {
//                var fromEmail = "sreenidhichidura@gmail.com";
//                var fromPassword = "zkoz tcjf zeoz xjjt"; // App password

//                var subject = $"Order Confirmation - HotByte #{order.Id}";
//                var body = $"Thank you for your order!\n\nOrder ID: {order.Id}\nPlaced On: {order.CreatedAt}\n\nItems:\n";

//                foreach (var item in order.OrderItems)
//                {
//                    var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
//                    body += $"- {menuItem?.Name ?? "Item"} x {item.Quantity} = ₹{item.Price}\n";
//                }

//                body += $"\nTotal Amount: ₹{order.TotalAmount}\n\nStatus: {order.Status}\n";
//                body += "\nWe will notify you once your order is confirmed.\n\n-- HotByte Team";

//                var smtpClient = new SmtpClient("smtp.gmail.com")
//                {
//                    Port = 587,
//                    Credentials = new NetworkCredential(fromEmail, fromPassword),
//                    EnableSsl = true
//                };

//                var mailMessage = new MailMessage(fromEmail, toEmail, subject, body);
//                await smtpClient.SendMailAsync(mailMessage);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Email sending failed: " + ex.Message);
//            }
//        }

//        private async Task SendOrderStatusUpdateEmail(string toEmail, Order order)
//        {
//            try
//            {
//                var fromEmail = "sreenidhichidura@gmail.com";
//                var fromPassword = "zkoz tcjf zeoz xjjt"; // App password

//                var subject = $"Order Update - HotByte #{order.Id}";
//                var body = $"Your order #{order.Id} status has been updated to: {order.Status}.\n\nThank you for choosing HotByte!";

//                var smtpClient = new SmtpClient("smtp.gmail.com")
//                {
//                    Port = 587,
//                    Credentials = new NetworkCredential(fromEmail, fromPassword),
//                    EnableSsl = true
//                };

//                var mailMessage = new MailMessage(fromEmail, toEmail, subject, body);
//                await smtpClient.SendMailAsync(mailMessage);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Email sending failed: " + ex.Message);
//            }
//        }
//    }

//    // DTO for updating order status
//    public class UpdateOrderStatusDto
//    {
//        public string Status { get; set; }
//    }
//}
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
    [Authorize(Roles = "User")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // User places order
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Invalid user.");

            if (dto == null || dto.Items == null || !dto.Items.Any())
                return BadRequest("Order items cannot be empty.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    BranchId = dto.BranchId,
                    PaymentMethod = dto.PaymentMethod,
                    Status = "Pending",
                    TotalAmount = 0,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var item in dto.Items)
                {
                    var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                    if (menuItem == null)
                        return BadRequest($"Invalid menu item ID: {item.MenuItemId}");

                    var discount = menuItem.DiscountPercentage ?? 0;
                    var priceAfterDiscount = menuItem.Price - (menuItem.Price * discount / 100);

                    var orderItem = new OrderItem
                    {
                        MenuItemId = menuItem.Id,
                        Quantity = item.Quantity,
                        Price = priceAfterDiscount * item.Quantity
                    };

                    order.TotalAmount += orderItem.Price;
                    order.OrderItems.Add(orderItem);
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Save to get Order.Id

                var shippingAddress = new ShippingAddress
                {
                    UserId = userId,
                    AddressLine = dto.ShippingAddress.AddressLine,
                    City = dto.ShippingAddress.City,
                    Pincode = dto.ShippingAddress.Pincode,
                    State = dto.ShippingAddress.State,
                    Country = dto.ShippingAddress.Country
                };

                _context.ShippingAddresses.Add(shippingAddress);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Send confirmation email to user
                if (User.Identity.IsAuthenticated)
                {
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (!string.IsNullOrEmpty(userEmail))
                        await SendOrderConfirmationEmail(userEmail, order);
                }

                var response = new OrderResponseDto
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        MenuItemId = oi.MenuItemId,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Failed to place order.", details = ex.ToString() });
            }
        }

        // User views own orders
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();

            var response = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
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

        private async Task SendOrderConfirmationEmail(string toEmail, Order order)
        {
            try
            {
                var fromEmail = "sreenidhichidura@gmail.com"; // Replace with your sender email
                var fromPassword = "zkoz tcjf zeoz xjjt"; // Replace with your app password

                var subject = $"Order Confirmation - HotByte #{order.Id}";
                var body = $"Thank you for your order!\n\nOrder ID: {order.Id}\nPlaced On: {order.CreatedAt}\n\nItems:\n";

                foreach (var item in order.OrderItems)
                {
                    var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                    body += $"- {menuItem?.Name ?? "Item"} x {item.Quantity} = ₹{item.Price}\n";
                }

                body += $"\nTotal Amount: ₹{order.TotalAmount}\n\nStatus: {order.Status}\n";
                body += "\nWe will notify you once your order is confirmed.\n\n-- HotByte Team";

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
}
