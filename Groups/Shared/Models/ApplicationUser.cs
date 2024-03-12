global using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string Name => $"{FirstName} {LastName}";
        public string? AvatarUrl { get; set; } = string.Empty;
        //This is not safe and is to be removed
        public string Password { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<Group>? Groups { get; set; }
    }
}
