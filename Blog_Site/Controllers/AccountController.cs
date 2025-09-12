using Blog_Site.Data;
using Blog_Site.Models.Concrete;
using Blog_Site.ModelView;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog_Site.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlogContext _context;
        public AccountController(BlogContext context) => _context = context;

        [HttpGet]
        public IActionResult Login()
        {
            return View(new ModelViews());
        }

        [HttpPost]
        public async Task<IActionResult> Login(ModelViews model)
        {
            if (ModelState.IsValid)
            {
                // Try to authenticate as a User
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.UserRegister.Email);
                if (user != null && user.PasswordHash == model.UserRegister.Password)
                {
                    var claims = new List<Claim> { new(ClaimTypes.Role, "User") };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Home");
                }

                // Try to authenticate as an Admin
                var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Email == model.AdminRegister.Email);
                if (admin != null && admin.PasswordHash == model.AdminRegister.Password)
                {
                    var claims = new List<Claim> { new(ClaimTypes.Role, "Admin") };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre!.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new ModelViews());
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(ModelViews model)
        {
            if (model.UserRegister != null)
            {
                // ModelState'i sadece UserRegister validasyonlarıyla sınırlamak
                ModelState.Clear();
                TryValidateModel(model.UserRegister);

                if (ModelState.IsValid)
                {
                    if (await _context.Users.AnyAsync(u => u.Email == model.UserRegister.Email))
                    {
                        ModelState.AddModelError("UserRegister.Email", "Bu e-posta adresi zaten kayıtlı.");
                        return View("Register", model);
                    }

                    var newUser = new User
                    {
                        Email = model.UserRegister.Email,
                        PasswordHash = model.UserRegister.Password,
                        UserName = model.UserRegister.UserName,
                        CreatedAt = DateTime.Now
                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login", "Account");
                }
            }
            return View("Register", model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(ModelViews model)
        {
            if (model.AdminRegister != null)
            {
                ModelState.Clear();
                TryValidateModel(model.AdminRegister);

                if (ModelState.IsValid)
                {
                    if (await _context.Admin.AnyAsync(a => a.Email == model.AdminRegister.Email))
                    {
                        ModelState.AddModelError("AdminRegister.Email", "Bu e-posta adresi zaten kayıtlı.");
                        return View("Register", model);
                    }

                    var newAdmin = new Admin
                    {
                        Email = model.AdminRegister.Email,
                        PasswordHash = model.AdminRegister.Password, 
                        Name = model.AdminRegister.Name,
                        CreatedAt = DateTime.Now
                    };
                    _context.Admin.Add(newAdmin);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login", "Account");
                }
            }
            return View("Register", model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}