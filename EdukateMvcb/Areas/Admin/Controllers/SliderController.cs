using EdukateMvcb.Contexts;
using EdukateMvcb.Models;
using EdukateMvcb.ViewModels.SliderVm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EdukateMvcb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        DataDbContext _db {  get; set; }
        IWebHostEnvironment _environment;

        public SliderController(DataDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Sliders.Select(c=> new SliderListItemVm
            {
                Id = c.Id,
                ImageUrl = c.ImageUrl,
                Profession = c.Profession,
                Instructor = c.Instructor,
            }).ToListAsync());
        }
        
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Cancel()
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            string filename = null;
            if (vm.FileImage != null)
            {
                filename = Guid.NewGuid() + Path.GetExtension(vm.FileImage.FileName);
                using (Stream fs = new FileStream(Path.Combine(_environment.WebRootPath, "Assets", "images", "stories", filename), FileMode.Create))
                {
                    await vm.FileImage.CopyToAsync(fs);
                }
            }
            Slider slider = new Slider()
            {
                Instructor = vm.Instructor,
                Profession = vm.Profession,
                ImageUrl = filename
            };
            _db.Sliders.AddAsync(slider);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            var data = await  _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
            return View(new SliderUpdateVm
            {
                ImageUrl = data.ImageUrl,
                Profession = data.Profession,
                Instructor = data.Instructor,
            });
        }
        [HttpPost]
        public async Task<IActionResult>Update(int? id,SliderUpdateVm vm)
        {
            if (id == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var data = await _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
            data.Instructor = vm.Instructor;
            data.Profession = vm.Profession;
            
                string filepath = Path.Combine(_environment.WebRootPath, "Assets", "images", "stories", data.ImageUrl);
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
             
            string filename = Guid.NewGuid() + Path.GetExtension(vm.FileImage.FileName);
            using (Stream fs = new FileStream(Path.Combine(_environment.WebRootPath, "Assets", "images", "stories", filename), FileMode.Create))
            {
                await vm.FileImage.CopyToAsync(fs);
            }
            data.ImageUrl = filename;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult>Delete(int? id)
        {
            if (id == null) return BadRequest();
            var data = await _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
            _db.Sliders.Remove(data);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
    }
}
