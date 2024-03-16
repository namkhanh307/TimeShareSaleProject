using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swp4RestWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swp4RestWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyDetailController : ControllerBase
    {
        private readonly PropertyDetail _context;

        public PropertyDetailController(PropertyDetail context)
        {
            _context = context;
        }

        // GET: api/PropertyDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDetail>>> GetPropertyDetails()
        {
            return await _context.PropertyDetails.ToListAsync();
        }

        // GET: api/PropertyDetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDetail>> GetPropertyDetail(int id)
        {
            var propertyDetail = await _context.PropertyDetails.FindAsync(id);

            if (propertyDetail == null)
            {
                return NotFound();
            }

            return propertyDetail;
        }
    }
}
