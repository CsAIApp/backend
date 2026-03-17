using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TeamLink.Core.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public int Budget { get; set; }
        public string? Category { get; set; }
        public List<string> Technologies { get; set; } = new();
        public List<string> CandidateQuestions { get; set; } = new();
        public string? ExternalLink { get; set; }

        public string OwnerId { get; set; }
        public AppUser Owner { get; set; }

    }
}
