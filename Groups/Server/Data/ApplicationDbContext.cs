using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Initialise the database with colors
            builder.Entity<Color>().HasData(
                new Color { Id = 1, Name = "Red", Hex = "#8AFF0000" },
                new Color { Id = 2, Name = "Green", Hex = "#8A00FF00" },
                new Color { Id = 3, Name = "Blue", Hex = "#8A0000FF" },
                new Color { Id = 4, Name = "Yellow", Hex = "#8AFFFF00" },
                new Color { Id = 5, Name = "Purple", Hex = "#8A800080" },
                new Color { Id = 6, Name = "Orange", Hex = "#8AFFA500" },
                new Color { Id = 7, Name = "Brown", Hex = "#8AA52A2A" },
                new Color { Id = 8, Name = "Pink", Hex = "#8AFFC0CB" },
                new Color { Id = 9, Name = "Black", Hex = "#8A000000" },
                new Color { Id = 10, Name = "White", Hex = "#8AFFFFFF" }
                );
            //Initialise the database with colors
            builder.Entity<GroupType>().HasData(
                new GroupType { Id = 1, Name = "Work", Image = "work" },
                new GroupType { Id = 2, Name = "Study", Image = "#study" },
                new GroupType { Id = 3, Name = "Library", Image = "#library" },
                new GroupType { Id = 4, Name = "Class", Image = "class" },
                new GroupType { Id = 5, Name = "Business", Image = "#business" },
                new GroupType { Id = 6, Name = "Other", Image = "#study" }
                );

        }
        public DbSet<Color> Colors { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<GroupEvent> GroupEvents { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }
    }


}
