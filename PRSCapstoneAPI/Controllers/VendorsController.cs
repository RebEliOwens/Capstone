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
    public class VendorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
          if (_context.Vendors == null)
          {
              return NotFound();
          }
            return await _context.Vendors.ToListAsync();
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
          if (_context.Vendors == null)
          {
              return NotFound();
          }
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

        [HttpGet("po/{vendorId}")]
        public async Task<ActionResult<Po>> CreatePo(int vendorId)
        {  
             Po po = new Po();
             po.Vendor = await _context.Vendors.FindAsync(vendorId);
             var results = (from v in _context.Vendors 
                            join p in _context.Products on v.Id equals p.VendorId 
                            join rl in _context.Requestlines on p.Id equals rl.ProductId
                            join r in _context.Requests on rl.RequestId equals r.Id 
                            where r.Status == "Approved" && v.Id == vendorId
                            select new
                            {
                                ProductId = p.Id,
                                Product = p.Name,
                                rl.Quantity,
                                p.Price,
                                LineTotal = p.Price * rl.Quantity
                            });
            var sortedLines = new SortedList<int, Poline>();

            foreach (var result in results)
            {
                if (!sortedLines.ContainsKey(result.ProductId))
                {
                    var poline = new Poline()
                    {
                        Product = result.Product,
                        Quantity = 0,
                        Price = result.Price,
                        LineTotal = 0
                    };
                    sortedLines.Add(result.ProductId, poline);
                }
                sortedLines[result.ProductId].Quantity += result.Quantity;
                sortedLines[result.ProductId].LineTotal += result.LineTotal;
            }
            po.Polines = sortedLines.Values.ToList();             //need to fix something in this method
            po.PoTotal = po.Polines.Sum(x => x.LineTotal);
           
            return po;
        }

        // PUT: api/Vendors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.Id)
            {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor)
        {
          if (_context.Vendors == null)
          {
              return Problem("Entity set 'AppDbContext.Vendors'  is null.");
          }
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendor", new { id = vendor.Id }, vendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            if (_context.Vendors == null)
            {
                return NotFound();
            }
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendorExists(int id)
        {
            return (_context.Vendors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
