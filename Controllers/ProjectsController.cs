using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TimeShareProject.Models;
using Project = TimeShareProject.Models.Project;


namespace TimeShareProject.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly TimeShareProjectContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProjectsController(TimeShareProjectContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        
        public IActionResult GetProject(int ID)
        {
            using TimeShareProjectContext context = new TimeShareProjectContext();
            List<int?> distinctBedTypes = @Common.GetDistinctBedTypes();
            ViewBag.DistinctBedTypes = distinctBedTypes;
            var items = context.Projects.FirstOrDefault(m => m.Id == ID);
            

            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }

        // GET: Projects
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project model, IFormFile AddressImage, IFormFile Image1, IFormFile Image2, IFormFile Image3)
        {
            var newProject = new Project
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Address = model.Address,
                TotalUnit = model.TotalUnit,
                GeneralDescription = model.GeneralDescription,
                DetailDescription = model.DetailDescription,
                Status = model.Status,
                Star = model.Star
            };
            newProject.AddressImage = SaveProjectImage(newProject, AddressImage).Result;
            newProject.Image1 = SaveProjectImage(newProject, Image1).Result;
            newProject.Image2 = SaveProjectImage(newProject, Image2).Result;
            newProject.Image3 = SaveProjectImage(newProject, Image3).Result;
            _context.Projects.Add(newProject);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

            //return View(model);
        }

        public async Task<string> SaveProjectImage(Project project, IFormFile imageFile)
        {
            if (project == null || imageFile == null || imageFile.Length == 0)
            {
                return null;
            }
            var projectShortName = project.ShortName;
            var fileName = Path.GetFileName(imageFile.FileName);
            var path = Path.Combine("img", projectShortName);
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, path);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = fileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return project.ShortName + "/" + fileName;
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, IFormFile AddressImage, IFormFile Image1, IFormFile Image2, IFormFile Image3)
        {
            var existingProject = await _context.Projects.FindAsync(id);
            try
            {
                existingProject.ShortName = project.ShortName;
                existingProject.Name = project.Name;
                existingProject.Address = project.Address;
                existingProject.TotalUnit = project.TotalUnit;
                existingProject.GeneralDescription = project.GeneralDescription;
                existingProject.DetailDescription = project.DetailDescription;
                existingProject.Status = project.Status;
                existingProject.Star = project.Star;

                if (AddressImage != null)
                {
                    existingProject.AddressImage = await SaveProjectImage(project, AddressImage);
                }
                if (Image1 != null)
                {
                    existingProject.Image1 = await SaveProjectImage(project, Image1);
                }
                if (Image2 != null)
                {
                    existingProject.Image2 = await SaveProjectImage(project, Image2);
                }
                if (Image3 != null)
                {
                    existingProject.Image3 = await SaveProjectImage(project, Image3);
                }
                _context.Update(existingProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred while updating the property.");
            }
            return View(project);
        }



        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }


        public IActionResult GetFeedback(int projectId)
        {
            
            var rates = _context.Rates.Where(r => r.ProjectId == projectId).ToList();
            return PartialView("_FeedbackPartial", rates);
        }
        public IActionResult Rates(int Id, string detailRate, int starRate)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var returnUrl = Url.Action("Rates", "Projects", new { Id, detailRate, starRate});
                return RedirectToAction("Login", "Login", new {returnUrl});
            }

            string username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Account.Username == username);

            if (user != null)
            {
                
                int userId = user.Id;

            
                var existingVote = _context.Rates.FirstOrDefault(r => r.ProjectId == Id && r.UserId == userId);

                if (existingVote != null)
                {
                  
                    existingVote.DetailRate = detailRate;
                    existingVote.StarRate = starRate;
                    _context.SaveChanges();

                    return RedirectToAction("GetProject", "Projects", new {Id});
                }
                else
                {
                   
                    TimeShareProjectContext context = new TimeShareProjectContext();
                    Rate rate = new Rate
                    {
                        ProjectId = Id,
                        UserId = userId,
                        DetailRate = detailRate,
                        StarRate = starRate,
                    };
                    context.Rates.Add(rate);
                    context.SaveChanges();

                    return RedirectToAction("GetProject", "Projects");
                }
            }
            else
            {
                
                return RedirectToAction("Login", "Login");
            }
        }
            

    }
}
