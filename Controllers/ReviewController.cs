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
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] ReviewDto dto)
        {
            var nameIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (nameIdClaim == null)
                return Unauthorized("User claim missing.");

            string userId = nameIdClaim.Value;

            var exists = await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.MenuItemId == dto.MenuItemId);

            if (exists) return BadRequest("You have already reviewed this item.");

            var review = new Review
            {
                UserId = userId,
                MenuItemId = dto.MenuItemId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            var response = new ReviewResponseDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                MenuItemId = review.MenuItemId,
                UserEmail = user?.Email ?? "User"
            };

            return Ok(response);
        }


        [HttpGet("item/{menuItemId}")]
        public async Task<IActionResult> GetReviews(int menuItemId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.MenuItemId == menuItemId)
                .Include(r => r.User)
                .ToListAsync();

            var response = reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                MenuItemId = r.MenuItemId,
                UserEmail = r.User?.Email ?? "User"
            }).ToList();

            return Ok(response);
        }

    }
}
