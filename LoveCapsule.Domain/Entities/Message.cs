using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoveCapsule.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }

        public List<Media> Medias { get; set; } = new List<Media>();
    }
}
