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
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "E-posta ve şifre zorunludur.");
                return View(new ModelViews());
            }

            // Önce User tablosunda ara
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == Password);
            if (user != null)
            {
                var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, "User")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Home");
            }

            // Admin tablosunda ara
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Email == Email && a.PasswordHash == Password);
            if (admin != null)
            {
                var claims = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Name),
            new(ClaimTypes.Email, admin.Email),
            new(ClaimTypes.Role, "Admin")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Home");
            }

            // Eğer bulunamadıysa hata
            ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre!");
            return View(new ModelViews());
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