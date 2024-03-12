namespace Server.Controllers
{
    [AllowAnonymous]
    public class GroupEventController : BaseController<GroupEvent>
    {
        public GroupEventController(ApplicationDbContext context) : base(context)
        {
        }
    }
}
