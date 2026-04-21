using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoveCapsule.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string HostName { get; set; }

        public string Slug { get; set; }

        public DateTime EventDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();
        public Guid UserId { get; set; }
    }
}
