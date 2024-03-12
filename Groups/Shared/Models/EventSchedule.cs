using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class EventSchedule : BaseModel
    {
        // Properties
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Description { get; set; }
        public int DayOfWeek { get; set; }
        // Navigation Properties
        public int VenueId { get; set; }
        public virtual Venue? Venue { get; set; }
        public int GroupEventId { get; set; }
        public virtual GroupEvent? GroupEvent { get; set; }

        [NotMapped]
        public string StartAndEndTime
        {
            get
            {
                return $"{StartDateTime.ToShortTimeString()} - {EndDateTime.ToShortTimeString()}";
            }
        }

        
    }
}
