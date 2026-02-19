using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeamLink.API.DTO;
using TeamLink.Core.Entities;
using TeamLink.Data;

namespace TeamLink.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // 1. KENDİ PROFİLİMİ GETİR (GET: api/users/me)
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(u => u.Skills)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    u.FullName,
                    Skills = u.Skills.Select(s => s.Name).ToList()
                    //buraya bio, link eklencek
                })
                .FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(user);
        }

        // 2. PROFİL GÜNCELLE (PUT: api/users/me)
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            user.FullName = request.FullName;
            // user.Bio = request.Bio;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil güncellendi" });
        }

        // 3. PROFİLİME YETENEK EKLE (POST: api/users/me/skills)
        [HttpPost("me/skills")]
        public async Task<IActionResult> AddSkillToProfile([FromBody] AddUserSkillDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(u => u.Skills)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var skill = await _context.Skills.FindAsync(request.SkillId);
            if (skill == null) return NotFound("Böyle bir yetenek bulunamadı.");

            if (user.Skills.Any(s => s.Id == request.SkillId))
                return BadRequest("Bu yetenek zaten profilinizde ekli.");

            user.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{skill.Name} profilinize eklendi." });
        }
    }

    // Önce DTO oluşturalım (Aynı dosyanın en altına veya DTO klasörüne koyabilirsin)
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Bio { get; set; }
}

// Controller'ın içine eklenecek Metod
[HttpPut("me")]
        [Authorize] // Sadece giriş yapmış kişiler erişebilir
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            // Giriş yapmış kullanıcının ID'sini Token'dan alıyoruz
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(userEmail); // _userManager yoksa DbContext ile de bulabilirsin
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            // Gelen yeni verileri kullanıcıya ata
            user.FullName = dto.FullName;
            user.Title = dto.Title;
            user.Bio = dto.Bio;

            // Veritabanına kaydet
            await _userManager.UpdateAsync(user);
            // veya _context.SaveChanges();

            return Ok(new { message = "Profil başarıyla güncellendi!" });
        }
    }