using HotByteAPI.Data;
using HotByteAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotByteAPI.Interfaces;
namespace HotByteAPI.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly AppDbContext _context;

        public BranchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<Branch> GetBranchByIdAsync(int id)
        {
            return await _context.Branches.FindAsync(id);
        }

        public async Task AddBranchAsync(Branch branch)
        {
            await _context.Branches.AddAsync(branch);
        }

        public async Task DeleteBranchAsync(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
                _context.Branches.Remove(branch);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}