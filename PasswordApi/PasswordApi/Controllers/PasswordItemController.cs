using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordApi.Models;

namespace PasswordApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordItemController : ControllerBase
    {
        private readonly PasswordContext _context;

        public PasswordItemController(PasswordContext context)
        {
            _context = context;
        }

        // GET: api/PasswordItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PasswordItem>>> GetPasswordItems()
        {
            return await _context.PasswordItems.ToListAsync();
        }

        // GET: api/PasswordItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PasswordItem>> GetPasswordItem(long id)
        {
            var passwordItem = await _context.PasswordItems.FindAsync(id);

            if (passwordItem == null)
            {
                return NotFound();
            }

            return passwordItem;
        }

        // PUT: api/PasswordItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPasswordItem(long id, PasswordItem passwordItem)
        {
            if (id != passwordItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(passwordItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PasswordItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PasswordItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PasswordItem>> PostPasswordItem(PasswordItem passwordItem)
        {
            _context.PasswordItems.Add(passwordItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPasswordItem), new { id = passwordItem.Id }, passwordItem);
        }

        // DELETE: api/PasswordItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePasswordItem(long id)
        {
            var passwordItem = await _context.PasswordItems.FindAsync(id);
            if (passwordItem == null)
            {
                return NotFound();
            }

            _context.PasswordItems.Remove(passwordItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PasswordItemExists(long id)
        {
            return _context.PasswordItems.Any(e => e.Id == id);
        }
    }
}
