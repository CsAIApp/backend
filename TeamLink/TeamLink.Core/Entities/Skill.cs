using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLink.Core.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AppUser> Users { get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}
