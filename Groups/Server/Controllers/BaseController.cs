namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class BaseController<T> : ControllerBase where T : BaseModel
    {
        public readonly ApplicationDbContext _context;
        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public virtual async Task<ActionResult<List<T>>> Get()
        {
            return _context.Set<T>().ToList();
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> Get(int id)
        {
            var model = await _context.Set<T>().FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return model;
        }

        [HttpPost]
        public virtual async Task<ActionResult<T>> Post(T model)
        {
            _context.Set<T>().Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<T>> Delete(int id)
        {
            var model = await _context.Set<T>().FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            _context.Set<T>().Remove(model);
            await _context.SaveChangesAsync();
            return model;
        }

        [HttpPut]
        public virtual async Task<ActionResult<T>> Put(T model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return model;
        }

    }
}
