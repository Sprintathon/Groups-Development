using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Group : BaseModel
    {
        // Properties
        public string Name { get; set; }
        public string Description { get; set; }
        public int GroupTypeId { get; set; }
        public GroupType? GroupType { get; set; }
        public int ColorId { get; set; }
        public Color? Color { get; set; }

        // Navigation Properties
        public virtual List<ApplicationUser>? Users { get; set; } = new();
        public virtual List<GroupEvent>? Events { get; set; } = new();
    }

    public class GroupType : BaseModel
    {
        public string Name { get; set; } = "Default";
        public string Image { get; set; } = "study";
    }

    public class Color  : BaseModel
    {
        public string Name { get; set; } = "Default";
        public string Hex { get; set; } = "#B5B5B5";
    }
}
