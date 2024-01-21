using EdukateMvcb.Contexts;
using EdukateMvcb.Models;
using EdukateMvcb.ViewModels.SliderVm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EdukateMvcb.Controllers
{
    public class HomeController : Controller
    {
        DataDbContext _db {  get; set; }

        public HomeController(DataDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult>  Index()
        {
            return View(await _db.Sliders.Select(c => new SliderListItemVm
            {
                Id = c.Id,
                ImageUrl = c.ImageUrl,
                Profession = c.Profession,
                Instructor = c.Instructor,
            }).ToListAsync());
        }
    }
}
