using System.Diagnostics;
using Blog_Site.DatabaseConnection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogContext _context;
        public HomeController(BlogContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var latestPost = await _context.Posts
                .OrderByDescending(p => p.PublishedDate)
                .Take(5)
                .ToListAsync();
            return View(latestPost);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
    }
}
