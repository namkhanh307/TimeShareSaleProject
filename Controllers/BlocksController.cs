using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeShareProject.Models;

namespace TimeShareProject.Controllers
{
    public class BlocksController : Controller
    {
        private readonly _4restContext _context;

        public BlocksController(_4restContext context)
        {
            _context = context;
        }

        // GET: Blocks
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blocks.ToListAsync());
        }




        [HttpPost]
        public IActionResult UpdateProportion(int id, double proportion)
        {
            var block = _context.Blocks.FirstOrDefault(b => b.Id == id);
            if (block != null)
            {
                block.Proportion = proportion;
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult AddBlock()
        {
            // Inside your controller action or service method
            using (var context = new _4restContext())
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1); // Start from January 1st of the current year

                for (int i = 1; i <= 53; i++)
                {
                    DateTime blockStartDate = startDate.AddDays((i - 1) * 7);
                    DateTime blockEndDate = blockStartDate.AddDays(6);

                    var block = new Block
                    {
                        Id = i,
                        BlockNumber = i,
                        StartDay = blockStartDate.Day,
                        StartMonth = blockStartDate.Month,
                        EndDay = blockEndDate.Day,
                        EndMonth = blockEndDate.Month,
                        Proportion = 100
                    };

                    context.Blocks.Add(block);
                    context.SaveChanges();
                }

                context.SaveChanges();
            }
            
            return RedirectToAction("Login");
        }
    }
}
