using HotByteAPI.Data;
using HotByteAPI.DTOs;
using HotByteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotByteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BranchController(AppDbContext context)
        {
            _context = context;
        }

        // Admin: Add/Delete branches
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBranch(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return Ok(branch);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return NotFound();
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // All roles: View/select branches
        [HttpGet]
        [Authorize] // All authenticated roles
        public IActionResult GetBranches()
        {
            return Ok(_context.Branches.ToList());
        }
    }
}
