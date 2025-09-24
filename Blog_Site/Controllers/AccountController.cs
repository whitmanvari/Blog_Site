using Blog_Site.Data;
using Blog_Site.Models.Concrete;
using Blog_Site.ModelView;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog_Site.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlogContext _context;

        public AccountController(BlogContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            HttpContext.Session.SetString("AdminName", "Hazal İlik");
            var sessionAdminName = HttpContext.Session.GetString("AdminName");

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                IsEssential = true
            };
            Response.Cookies.Append("AdminName", "Hazal İlik", cookieOptions);
            var cookieAdminName = Request.Cookies["AdminName"];

            ViewBag.SessionAdminName = sessionAdminName;
            ViewBag.CookieAdminName = cookieAdminName;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new ModelViews());
        }

        [HttpPost]
        public async Task<IActionResult> Login(ModelViews model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var email = model.UserRegister?.Email; //Login için modeldeki emaili çekeriz.
            var password = model.UserRegister?.Password; //Login için modeldeki şifreyi çekeriz.
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "E-mail or Password cannot be empty!");
                return View(model);
            }
            //User Login Control (created a new Claim object. It will arrange user's authentication and authorization )
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
            if (user != null)
            {
                //ıt comes from system.security.claims
                //[Authorize(Roles="User")] --> we can use it like this.

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, "User")
                };
                //asp.net core authentication cookie scheme
                //claimsIdentity comes from system.security.claims
                //CookieAuthanticationDefaults comes from Microsoft.AspNetCore.Authentication.Cookies
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Home");
            }

            //Admin Login Control
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Email == email && a.PasswordHash == password);
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
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
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