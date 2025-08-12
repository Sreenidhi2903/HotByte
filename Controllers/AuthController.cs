using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotByteAPI.Data;
using HotByteAPI.Models;
using HotByteAPI.Services;
using HotByteAPI.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace HotByteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, ITokenService tokenService, IConfiguration config)
        {
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.Email == dto.Email.ToLower()))
                return BadRequest("Email already registered");

            if (dto.Role.ToLower() == "admin")
            {
                if (dto.RegistrationSecret != _config["Jwt:Key"])
                    return Unauthorized("Invalid admin registration secret");
            }

            if (dto.Role.ToLower() == "restaurant" && string.IsNullOrEmpty(dto.City))
            {
                return BadRequest("City is required for restaurant registration");
            }

            using var hmac = new HMACSHA512();

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key,
                Role = dto.Role,
                City = dto.City?.Trim()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { token = _tokenService.CreateToken(user) });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email.ToLower());
            if (user == null) return Unauthorized("Invalid credentials");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for (int i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid credentials");

            return Ok(new { token = _tokenService.CreateToken(user), role = user.Role });
        }
    }
}
