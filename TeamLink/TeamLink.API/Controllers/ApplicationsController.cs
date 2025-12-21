using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeamLink.API.DTOs;
using TeamLink.Core.Entities;
using TeamLink.Data;

namespace TeamLink.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApplicationsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. BAŞVURU YAP (POST: api/applications)
        [HttpPost]
        public async Task<IActionResult> ApplyToProject(CreateApplicationDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Giriş yapan kişi

            var project = await _context.Projects.FindAsync(request.ProjectId);
            if (project == null) return NotFound("Proje bulunamadı.");

            if (project.OwnerId == userId)
                return BadRequest("Kendi projenize başvuramazsınız.");

            var existingApp = await _context.Applications
                .FirstOrDefaultAsync(a => a.ProjectId == request.ProjectId && a.ApplicantId == userId);

            if (existingApp != null)
                return BadRequest("Bu projeye zaten başvurunuz var.");

            // d. Kaydet
            var application = new Application
            {
                ProjectId = request.ProjectId,
                ApplicantId = userId,
                Message = request.Message,
                Status = ApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Başvuru başarıyla alındı." });
        }

        // 2. BAŞVURULARIM (GET: api/applications/my-applications)
        [HttpGet("my-applications")]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetMyApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var applications = await _context.Applications
                .Include(a => a.Project)
                .Where(a => a.ApplicantId == userId)
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new ApplicationDto
                {
                    Id = a.Id,
                    ProjectId = a.ProjectId,
                    ProjectTitle = a.Project.Title,
                    Status = a.Status.ToString(),
                    AppliedAt = a.AppliedAt,
                    Message = a.Message
                })
                .ToListAsync();

            return Ok(applications);
        }

        // 3. PROJEME GELEN BAŞVURULAR (GET: api/applications/project/{id})
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetProjectApplications(int projectId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();
            if (project.OwnerId != userId) return Forbid();

            var applications = await _context.Applications
                .Include(a => a.Applicant)
                .Where(a => a.ProjectId == projectId)
                .Select(a => new ApplicationDto
                {
                    Id = a.Id,
                    ApplicantName = a.Applicant.FullName ?? a.Applicant.UserName,
                    Message = a.Message,
                    Status = a.Status.ToString(),
                    AppliedAt = a.AppliedAt
                })
                .ToListAsync();

            return Ok(applications);
        }
    }
}