//using HotByteAPI.Data;
//using HotByteAPI.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace HotByteAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize(Roles = "Admin")]  // only admins can manage branches
//    public class AdminController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public AdminController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Admin/branches
//        [HttpGet("branches")]
//        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
//        {
//            return await _context.Branches.ToListAsync();
//        }

//        // GET: api/Admin/branches/{id}
//        [HttpGet("branches/{id}")]
//        public async Task<ActionResult<Branch>> GetBranch(int id)
//        {
//            var branch = await _context.Branches.FindAsync(id);
//            if (branch == null)
//                return NotFound();

//            return branch;
//        }

//        // POST: api/Admin/branches
//        [HttpPost("branches")]
//        public async Task<ActionResult<Branch>> CreateBranch(Branch branch)
//        {
//            _context.Branches.Add(branch);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetBranch), new { id = branch.Id }, branch);
//        }

//        // PUT: api/Admin/branches/{id}
//        [HttpPut("branches/{id}")]
//        public async Task<IActionResult> UpdateBranch(int id, Branch branch)
//        {
//            if (id != branch.Id)
//                return BadRequest();

//            _context.Entry(branch).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!BranchExists(id))
//                    return NotFound();
//                else
//                    throw;
//            }

//            return NoContent();
//        }

//        // DELETE: api/Admin/branches/{id}
//        [HttpDelete("branches/{id}")]
//        public async Task<IActionResult> DeleteBranch(int id)
//        {
//            var branch = await _context.Branches.FindAsync(id);
//            if (branch == null)
//                return NotFound();

//            _context.Branches.Remove(branch);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool BranchExists(int id)
//        {
//            return _context.Branches.Any(e => e.Id == id);
//        }
//    }
//}
using HotByteAPI.Data;
using HotByteAPI.DTOs;
using HotByteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotByteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Admin/branches
        [HttpGet("branches")]
        public async Task<ActionResult<IEnumerable<BranchCreateDto>>> GetBranches()
        {
            var branches = await _context.Branches
                .Where(b => !b.IsDeleted) // Only non-deleted
                .Select(b => new BranchCreateDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Location = b.Location,
                    ContactNumber = b.ContactNumber
                })
                .ToListAsync();

            return Ok(branches);
        }

        // GET: api/Admin/branches/{id}
        [HttpGet("branches/{id}")]
        public async Task<ActionResult<BranchCreateDto>> GetBranch(int id)
        {
            var branch = await _context.Branches
                .Where(b => b.Id == id && !b.IsDeleted) // Only non-deleted
                .Select(b => new BranchCreateDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Location = b.Location,
                    ContactNumber = b.ContactNumber
                })
                .FirstOrDefaultAsync();

            if (branch == null)
                return NotFound();

            return Ok(branch);
        }

        // POST: api/Admin/branches
        [HttpPost("branches")]
        public async Task<ActionResult<BranchCreateDto>> CreateBranch(BranchCreateDto dto)
        {
            var branch = new Branch
            {
                Name = dto.Name,
                Location = dto.Location,
                ContactNumber = dto.ContactNumber,
                IsDeleted = false  // Default to not deleted
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            var branchDto = new BranchCreateDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Location = branch.Location,
                ContactNumber = branch.ContactNumber
            };

            return CreatedAtAction(nameof(GetBranch), new { id = branch.Id }, branchDto);
        }

        // PUT: api/Admin/branches/{id}
        [HttpPut("branches/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, BranchCreateDto dto)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null || branch.IsDeleted)
                return NotFound();

            branch.Name = dto.Name;
            branch.Location = dto.Location;
            branch.ContactNumber = dto.ContactNumber;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Admin/branches/{id}
        [HttpDelete("branches/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null || branch.IsDeleted)
                return NotFound();

            branch.IsDeleted = true;  // Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

