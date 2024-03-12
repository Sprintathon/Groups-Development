global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Server.Data;
global using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ApplicationUserController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        public ApplicationUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public virtual async Task<ActionResult<List<ApplicationUser>>> Get()
        {
            return _context.Set<ApplicationUser>().ToList();
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<ApplicationUser>> Get(string id)
        {
            var model = _context.Set<ApplicationUser>()
                .Include(x => x.Groups).ThenInclude(x => x.GroupType)
                .Include(x => x.Groups).ThenInclude(x => x.Color)
                .Include(u=>u.Groups).ThenInclude(g=>g.Events).ThenInclude(e=>e.Schedules).ThenInclude(s=>s.Venue)
                .FirstOrDefault(u => u.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            return model;
        }

        [HttpPost]
        public virtual async Task<ActionResult<ApplicationUser>> Post(ApplicationUser model)
        {
            try
            {

                _context.Set<ApplicationUser>().Add(model);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<ApplicationUser>> Delete(string id)
        {
            var model = await _context.Set<ApplicationUser>().FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            _context.Set<ApplicationUser>().Remove(model);
            await _context.SaveChangesAsync();
            return model;
        }

        [HttpDelete]
        public virtual async Task<ActionResult> Delete() 
        {             
            _context.Set<ApplicationUser>().RemoveRange(_context.Set<ApplicationUser>());
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public virtual async Task<ActionResult<ApplicationUser>> Put(ApplicationUser model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return model;
        }

        [HttpGet]
        [Route("login/{userName}/{password}")]
        public async Task<ActionResult<ApplicationUser>> Login(string username, string password)
        {
            var user = await _context.Set<ApplicationUser>().FirstOrDefaultAsync(x => (x.UserName == username || x.Email == username) && x.Password == password);
            if (user == null)
            {
                return BadRequest();
            }
            return user;
        }

        [HttpGet("AddTogroup/{userId}/{groupId}")]
        public async Task<ActionResult<ApplicationUser>> AddToGroup(string userId, int groupId)
        {
            var user = _context.Set<ApplicationUser>()
                .Include(x=>x.Groups).ThenInclude(x => x.GroupType)
                .Include(x => x.Groups).ThenInclude(x => x.Color).FirstOrDefault(u =>u.Id == userId);
            var group = await _context.Set<Group>().FindAsync(groupId);
            if (user == null || group == null)
            {
                return BadRequest();
                //return NotFound();
            }
            //check if user is already in group
            if (user.Groups.Contains(group))
            {
                return user;
            }
            user.Groups.Add(group);
            await _context.SaveChangesAsync();
            return user;
        }

        [HttpGet("RemoveFromGroup/{userId}/{groupId}")]
        public async Task<ActionResult<ApplicationUser>> RemoveFromGroup(string userId, int groupId)
        {
            var user = _context.Set<ApplicationUser>().Include(x => x.Groups).FirstOrDefault(u => u.Id == userId);
            var group = await _context.Set<Group>().FindAsync(groupId);
            if (user == null || group == null)
            {
                return NotFound();
            }
            //check if user is already in group
            if (!user.Groups.Contains(group))
            {
                return user;
            }
            user.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
