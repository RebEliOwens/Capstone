using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSCapstoneAPI.Models;

namespace PRSCapstoneAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestLinesController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<IActionResult> RecalculateRequestTotal(int requestId)
        { 
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null)
            { 
                return NotFound();
            }
            request.Total = (from rl in _context.Requestlines join p in _context.Products on rl.ProductId equals p.Id where rl.RequestId == requestId select new { Total = rl.Quantity * p.Price }).Sum(x => x.Total);
            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // GET: api/RequestLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestlines()
        {
          if (_context.Requestlines == null)
          {
              return NotFound();
          }
            return await _context.Requestlines.Include(x => x.Product).ToListAsync();
        }

        // GET: api/RequestLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id)
        {
          if (_context.Requestlines == null)
          {
              return NotFound();
          }
            var requestLine = await _context.Requestlines.FindAsync(id);

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }

        // PUT: api/RequestLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestLine)
        {
            if (id != requestLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(requestLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await RecalculateRequestTotal(requestLine.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestLineExists(id))
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

        // POST: api/RequestLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine)
        {
          if (_context.Requestlines == null)
          {
              return Problem("Entity set 'AppDbContext.Requestlines'  is null.");
          }
            _context.Requestlines.Add(requestLine);
            await _context.SaveChangesAsync();
            await RecalculateRequestTotal(requestLine.RequestId);

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: api/RequestLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestLine(int id)
        {
            if (_context.Requestlines == null)
            {
                return NotFound();
            }
            var requestLine = await _context.Requestlines.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            _context.Requestlines.Remove(requestLine);
            await _context.SaveChangesAsync();
            await RecalculateRequestTotal(requestLine.RequestId);

            return NoContent();
        }

        private bool RequestLineExists(int id)
        {
            return (_context.Requestlines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
