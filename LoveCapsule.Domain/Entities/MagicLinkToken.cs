using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoveCapsule.Domain.Entities
{
    public class MagicLinkToken
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public string TokenHash { get; set; } 

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }
    }
}
