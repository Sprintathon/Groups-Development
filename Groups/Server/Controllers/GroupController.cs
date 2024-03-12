
namespace Server.Controllers
{
    [AllowAnonymous]
    public class GroupController : BaseController<Group>
    {
        public GroupController(ApplicationDbContext context) : base(context)
        {
        }

        [HttpGet]
        public override async Task<ActionResult<List<Group>>> Get() => _context.Groups.Include(x => x.GroupType).Include(x=>x.Color).ToList();
    }
}
