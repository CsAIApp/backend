using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/projects
        // Tüm aktif ilanları getirir. Herkes görebilir (Authorize yok).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Owner)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    OwnerName = p.Owner.FullName ?? p.Owner.UserName,
                    CreatedAt = p.CreatedAt,
                    Budget = p.Budget,
                    Category = p.Category,
                    Technologies = p.Technologies,
                    CandidateQuestions = p.CandidateQuestions,
                    ExternalLink = p.ExternalLink
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET: api/projects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound(new { message = "Proje bulunamadı." });
            }

            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                OwnerName = project.Owner?.FullName ?? project.Owner?.UserName ?? "İsimsiz",
                CreatedAt = project.CreatedAt,
                Budget = project.Budget,
                Category = project.Category,
                Technologies = project.Technologies,
                CandidateQuestions = project.CandidateQuestions,
                ExternalLink = project.ExternalLink
            };

            return Ok(projectDto);
        }

        // POST: api/projects
        // Yeni ilan ekler. SADECE GİRİŞ YAPANLAR (Kilitli).
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var project = new Project
            {
                Title = request.Title,
                Description = request.Description,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                Budget = request.Budget,
                Category = request.Category,
                Technologies = request.Technologies ?? new List<string>(),
                CandidateQuestions = request.CandidateQuestions ?? new List<string>(),
                ExternalLink = request.ExternalLink
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Proje başarıyla oluşturuldu", projectId = project.Id });
        }
    }
}
