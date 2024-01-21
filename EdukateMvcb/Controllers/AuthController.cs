using EdukateMvcb.Helpers;
using EdukateMvcb.Models;
using EdukateMvcb.ViewModels.AuthVm;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EdukateMvcb.Controllers
{
    public class AuthController : Controller
    {
       

        SignInManager<AppUser> _signInManager {  get; set; }
        UserManager<AppUser> _userManager { get; set; }
        RoleManager<IdentityRole> _roleManager {  get; set; }
        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Register(RegisterVm vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }
            var user = new AppUser()
            {
                Email = vm.Email,
                Surname = vm.Surname,
                Name = vm.Name,
                UserName = vm.Username
            };
            var result = await _userManager.CreateAsync(user,vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View();
                }
            }
            return View();
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Login(string? returnUrl,LoginVm vm)
        {
            AppUser user;
            if(!ModelState.IsValid)
            {
                return View(vm);
            }
            if (vm.UsernameorEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(vm.UsernameorEmail);
            }
            else
            {
                user= await _userManager.FindByNameAsync(vm.UsernameorEmail);
            }
            if(user != null) 
            {
                var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.isRemember, true);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Invalid Login attempt");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Login Not Found");
                return View();
            }
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<bool> CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = item.ToString(),
                    });
                }
                return true;
            }
            return false;
        }

    }
}
