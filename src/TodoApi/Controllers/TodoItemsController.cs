using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(TodoContext context, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                _logger.LogWarning($"Joku yritti hakea TodoItemia ID:llä {id}, mutta sitä ei löytynyt.");
                return NotFound();
            }

            _logger.LogInformation($"TodoItem haettiin onnistuneesti ID:llä {id}.");
            return todoItem;
        }
        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                _logger.LogWarning($"Käyttäjä yritti päivittää TodoItemia ID:llä {id}, mutta ID ei vastannut pyynnön dataa ({todoItem.Id}).");
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"TodoItem ID:llä {id} päivitettiin onnistuneesti.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    _logger.LogWarning($"TodoItem ID:llä {id} ei löytynyt, päivitys epäonnistui.");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Tuntematon virhe päivitettäessä TodoItemia ID:llä {id}.");
                    throw;
                }
            }

            return NoContent();
        }
        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Uusi TodoItem luotu ID:llä {todoItem.Id}.");

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }
        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                _logger.LogWarning($"Joku yritti poistaa TodoItemin ID:llä {id}, mutta sitä ei löytynyt.");
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"TodoItem ID:llä {id} poistettiin onnistuneesti.");

            return NoContent();
        }
        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
