using HotByteAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HotByteAPI.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<IEnumerable<MenuItem>> GetMenuItemsByBranchAsync(int branchId);
        Task<MenuItem> GetMenuItemByIdAsync(int id);
        Task AddMenuItemAsync(MenuItem item);
        Task UpdateMenuItemAsync(MenuItem item);
        Task DeleteMenuItemAsync(int id);
        Task SaveChangesAsync();
    }
}