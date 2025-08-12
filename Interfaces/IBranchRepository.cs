using HotByteAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HotByteAPI.Interfaces
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Branch>> GetAllBranchesAsync();
        Task<Branch> GetBranchByIdAsync(int id);
        Task AddBranchAsync(Branch branch);
        Task DeleteBranchAsync(int id);
        Task SaveChangesAsync();
    }
}