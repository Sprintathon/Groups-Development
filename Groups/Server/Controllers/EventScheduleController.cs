namespace Server.Controllers
{
    [AllowAnonymous]
    public class EventScheduleController : BaseController<EventSchedule>
    {
        public EventScheduleController(ApplicationDbContext context) : base(context)
        {
        }
    }
    [AllowAnonymous]
    public class GroupTypeController : BaseController<GroupType>
    {
        public GroupTypeController(ApplicationDbContext context) : base(context)
        {
        }
    }
    [AllowAnonymous]
    public class ColorController : BaseController<Color>
    {
        public ColorController(ApplicationDbContext context) : base(context)
        {
        }
    }
}
