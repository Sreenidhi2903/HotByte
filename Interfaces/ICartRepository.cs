using HotByteAPI.Models;
using System.Threading.Tasks;
namespace HotByteAPI.Interfaces
{

    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId); // was int
        Task AddCartAsync(Cart cart);
        Task SaveChangesAsync();
    }

}