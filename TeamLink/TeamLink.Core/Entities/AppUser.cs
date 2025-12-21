using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TeamLink.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = "**";
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
