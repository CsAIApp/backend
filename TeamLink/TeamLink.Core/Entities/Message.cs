using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLink.Core.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public string SenderId { get; set; }
        public AppUser Sender { get; set; }

        public string ReceiverId { get; set; }
        public AppUser Receiver { get; set; }

        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
