using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                .Include(p => p.Owner) // Kullanıcı bilgisini de çek (JOIN atar)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt) // En yeni en üstte
                .Select(p => new ProjectDto // DTO'ya çevir
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    OwnerName = p.Owner.FullName ?? p.Owner.UserName, // Adı yoksa nickini koy
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(projects);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            // Veritabanından ID'si eşleşen ilk projeyi bul (Kullanıcı bilgisiyle birlikte)
            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);

            // Eğer proje yoksa 404 dön
            if (project == null)
            {
                return NotFound(new { message = "Proje bulunamadı." });
            }

            // Proje bulunduysa DTO'ya çevir ve Frontend'e yolla
            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                OwnerName = project.Owner?.FullName ?? project.Owner?.UserName ?? "İsimsiz",
                CreatedAt = project.CreatedAt

                // NOT: Eğer ProjectDto ve veritabanı modelinde aşağıdaki alanlar varsa 
                // başlarındaki "//" işaretini kaldırarak onları da Frontend'e gönderebilirsin:

                // Budget = project.Budget,
                // Category = project.Category,
                // ApplicationsCount = project.Applications?.Count ?? 0
            };

            return Ok(projectDto);
        }

        // POST: api/projects
        // Yeni ilan ekler. SADECE GİRİŞ YAPANLAR (Kilitli).
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto request)
        {
            // 1. Token'dan "Kim giriş yapmış?" bilgisini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // 2. Yeni Proje Nesnesi Oluştur
            var project = new Project
            {
                Title = request.Title,
                Description = request.Description,
                OwnerId = userId, // Token'dan gelen ID'yi yapıştır
                CreatedAt = DateTime.UtcNow
            };

            // 3. Kaydet
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Proje başarıyla oluşturuldu", projectId = project.Id });
        }
    }
}
