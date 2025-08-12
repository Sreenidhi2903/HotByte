using HotByteAPI.Data;
using HotByteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotByteAPI.Interfaces;
namespace HotByteAPI.Repositories
{

    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly AppDbContext _context;

        public MenuItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemsByBranchAsync(int branchId)
        {
            return await _context.MenuItems.Where(m => m.BranchId == branchId).ToListAsync();
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            return await _context.MenuItems.FindAsync(id);
        }

        public async Task AddMenuItemAsync(MenuItem item)
        {
            await _context.MenuItems.AddAsync(item);
        }

        public async Task UpdateMenuItemAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item != null)
                _context.MenuItems.Remove(item);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}