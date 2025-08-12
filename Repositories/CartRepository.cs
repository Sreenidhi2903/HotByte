using HotByteAPI.Data;
using HotByteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HotByteAPI.Interfaces;

namespace HotByteAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)  // <-- changed from int to string
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}