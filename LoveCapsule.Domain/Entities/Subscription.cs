using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoveCapsule.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public PlanType Plan { get; set; } 
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
