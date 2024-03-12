using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Venue : BaseModel
    {
        // Properties
        public string Name { get; set; }
        public string Address { get; set; }
        public string AvatarUrl { get; set; }

        // Navigation Properties
        public virtual List<EventSchedule>? Schedules { get; set; }
    }
}
