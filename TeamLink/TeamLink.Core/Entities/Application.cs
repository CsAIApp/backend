using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLink.Core.Entities
{
    public class Application
    {
        public int Id { get; set; }

        // İlişkiler
        public string ApplicantId { get; set; }
        public AppUser Applicant { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string? Message { get; set; }

        //0=Bekliyor, 1=Kabul, 2=Red
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ApplicationStatus { Pending, Accepted, Rejected }
}
