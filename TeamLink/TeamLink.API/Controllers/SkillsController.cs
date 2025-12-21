using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamLink.API.DTO;
using TeamLink.API.DTOs;
using TeamLink.Core.Entities;
using TeamLink.Data;

namespace TeamLink.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. TÜM YETENEKLERİ LİSTELE (GET: api/skills)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills()
        {
            return await _context.Skills
                .Select(s => new SkillDto { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }

        // 2. YENİ YETENEK EKLE (POST: api/skills)
        [HttpPost]
        public async Task<ActionResult<Skill>> CreateSkill([FromBody] SkillDto request)
        {
            if (await _context.Skills.AnyAsync(s => s.Name == request.Name))
                return BadRequest("Bu yetenek zaten ekli.");

            var skill = new Skill { Name = request.Name };
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return Ok(skill);
        }
    }
}