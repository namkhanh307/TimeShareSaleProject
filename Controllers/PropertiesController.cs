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
    public class PropertiesController : Controller
    {
        private readonly TimeShareProjectContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PropertiesController(TimeShareProjectContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }


    
        public IActionResult GetProperty(int ID)
        {
            using TimeShareProjectContext context = new();
            var property = context.Properties.FirstOrDefault(m => m.Id == ID);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }


        //[HttpPost]


        //public IActionResult FilterProperties(int projectId, int blockSelect, int bedSelect, string saleStatus)
        //{

        //    bool isReserve = saleStatus == "Reserve";
        //    bool isBuyNow = saleStatus == "Buynow";

        //    var properties = _context.Properties
        //                    .Where(p => p.Beds == bedSelect && p.ProjectId == projectId)
        //                    .ToList();

        //    var availableProperties = new List<Property>();
        //    foreach (var property in properties)
        //    {
        //        bool isSale = property.SaleDate <= DateTime.Today;
        //        bool isReservation = _context.Reservations.Any(r => r.PropertyId == property.Id && r.Block.Id == blockSelect);

        //        if ((isReserve && !isSale) || (isBuyNow && !isReservation && isSale))
        //        {
        //            availableProperties.Add(property);
        //        }
        //    }

        //    ViewBag.ProjectId = projectId;
        //    ViewBag.BlockSelect = blockSelect;
        //    ViewBag.SaleStatus = saleStatus;
        //    ViewBag.BedSelect = bedSelect;

        //    return View(availableProperties);
        //}
        // GET: Properties
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Index()
        {
            var timeShareProjectContext = _context.Properties.Include(p => p.Project);
            return View(await timeShareProjectContext.ToListAsync());
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            ViewBag.Projects = _context.Projects
                     .Select(p => new SelectListItem
                     {
                         Value = p.Id.ToString(),
                         Text = p.Name
                     })
                     .ToList();
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Property property, IFormFile ViewImage, IFormFile FrontImage, IFormFile InsideImage, IFormFile SideImage)
        {
            if (property.ProjectId == null)
            {
                return RedirectToAction("Error");
            }
            var newProperty = new Property
            {
                Id = property.Id,
                Name = property.Name,
                SaleStatus = property.SaleStatus,
                SaleDate = property.SaleDate,
                UnitPrice = property.UnitPrice,
                Beds = property.Beds,
                Occupancy = property.Occupancy,
                Bathroom = property.Bathroom,
                Views = property.Views,
                UniqueFeature = property.UniqueFeature,
                Size = property.Size,
                Status = property.Status,
                ProjectId = property.ProjectId,
            };
            newProperty.ViewImage = SavePropertyImage(property, ViewImage).Result;
            newProperty.FrontImage = SavePropertyImage(property, FrontImage).Result;
            newProperty.InsideImage = SavePropertyImage(property, InsideImage).Result;
            newProperty.SideImage = SavePropertyImage(property, SideImage).Result;

            _context.Properties.Add(newProperty);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public async Task<string> SavePropertyImage(Property property, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }
            var projectShortName = Common.GetProjectShortNameFromProperty(property);
            var propertyName = property.Name;
            var fileName = Path.GetFileName(imageFile.FileName); ;

            var path = Path.Combine("img", projectShortName, propertyName);
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, path);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return Path.Combine(projectShortName, propertyName, fileName).Replace("\\", "/");
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Projects = _context.Projects
         .Select(p => new SelectListItem
         {
             Value = p.Id.ToString(),
             Text = p.Name
         })
         .ToList();
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", @property.ProjectId);
            return View(@property);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property, IFormFile ViewImage, IFormFile FrontImage, IFormFile InsideImage, IFormFile SideImage)
        {
            var existingProperty = await _context.Properties.FindAsync(id);
            try
            {
                existingProperty.Name = property.Name;
                existingProperty.SaleStatus = property.SaleStatus;
                existingProperty.Status = property.Status;
                existingProperty.SaleDate = property.SaleDate;
                existingProperty.UnitPrice = property.UnitPrice;
                existingProperty.Beds = property.Beds;
                existingProperty.Occupancy = property.Occupancy;
                existingProperty.Size = property.Size;
                existingProperty.Bathroom = property.Bathroom;
                existingProperty.Views = property.Views;
                existingProperty.UniqueFeature = property.UniqueFeature;
                existingProperty.ProjectId = property.ProjectId;


                if (ViewImage != null)
                {
                    existingProperty.ViewImage = await SavePropertyImage(property, ViewImage);
                }
                if (FrontImage != null)
                {
                    existingProperty.FrontImage = await SavePropertyImage(property, FrontImage);
                }
                if (InsideImage != null)
                {
                    existingProperty.InsideImage = await SavePropertyImage(property, InsideImage);
                }
                if (SideImage != null)
                {
                    existingProperty.SideImage = await SavePropertyImage(property, SideImage);
                }
                _context.Update(existingProperty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred while updating the property.");
            }
            return View(property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @property = await _context.Properties.FindAsync(id);
            if (@property != null)
            {
                _context.Properties.Remove(@property);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
        public PartialViewResult ManageProperties(int? saleStatus, int? status, int? project, int? bed, int? saleDate)
        {
            var query = _context.Properties.AsQueryable();

            // Apply filters based on user-selected parameters
            if (saleStatus.HasValue)
            {
                bool saleStatusBool = saleStatus == 1;
                query = query.Where(p => p.SaleStatus == saleStatusBool);
            }

            if (status.HasValue)
            {
                bool statusBool = status == 1;
                query = query.Where(p => p.Status == statusBool);
            }

            if (project.HasValue)
            {
                query = query.Where(p => p.ProjectId == project);
            }

            if (bed.HasValue)
            {
                query = query.Where(p => p.Beds == bed);
            }

            if (saleDate.HasValue)
            {
                if (saleDate == 1)
                {
                    query = query.Where(p => p.SaleDate > DateTime.Now);
                }
                else
                {
                    query = query.Where(p => p.SaleDate < DateTime.Now);
                }

            }

            var filteredProperties = query.ToList();
            ViewBag.FilteredProperties = filteredProperties;


            return PartialView("_FilteredProperties", filteredProperties);
        }

        [HttpPost]
        public IActionResult SetSaleDate(DateTime saleDate, List<int> selectedProperties)
        {
            if (ModelState.IsValid)
            {
                // Ensure that at least one property is selected
                if (selectedProperties != null && selectedProperties.Any())
                {
                    // Process the selected properties and set the sale date
                    foreach (int propertyId in selectedProperties)
                    {
                        var property = _context.Properties.Find(propertyId);
                        if (property != null)
                        {
                            property.SaleDate = saleDate;
                        }
                    }

                    // Save changes to the database
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Properties"); // Redirect to properties index or another suitable page
                }
                else
                {
                    // Handle the case where no properties are selected
                    ModelState.AddModelError("", "Please select at least one property.");
                }
            }

            // If ModelState is not valid or no properties are selected, return to the form
            return RedirectToAction("Index", "Properties"); // Redirect to properties index or another suitable page
        }

        [HttpPost]
        public IActionResult FilterProperties(int projectId, int? blockSelect, int? bedSelect, string saleStatus)
        {
            var query = _context.Properties
                .Where(p => p.ProjectId == projectId);

            foreach (var property in query)
            {

                if (bedSelect != null)
                {
                    query = query.Where(property => property.Beds == bedSelect);
                }

                if (!string.IsNullOrEmpty(saleStatus))
                {
                    bool isReserve = saleStatus == "Reserve";
                    bool isBuyNow = saleStatus == "Buy now";

                    if (blockSelect != null)
                    {
                        if (isReserve)
                        {
                            query = query.Where(property => property.SaleDate > DateTime.Now);
                        }

                        else if (isBuyNow)
                        {
                            query = query.Where(property => !_context.Reservations.Any(r => r.PropertyId == property.Id && r.Block.Id == blockSelect));
                            query = query.Where(property => property.SaleDate < DateTime.Now);

                        }
                    }
                }
            }
            var availableProperties = query.ToList();

            ViewBag.ProjectId = projectId;
            ViewBag.BlockSelect = blockSelect;
            ViewBag.SaleStatus = saleStatus;
            ViewBag.BedSelect = bedSelect;

            return View(availableProperties);
        }
    }
}
