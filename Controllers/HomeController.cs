using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Data;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalWorkouts = await _context.Workouts.CountAsync(),
                TotalLogs = await _context.WorkoutLogs.CountAsync(),
                RecentLogs = await _context.WorkoutLogs
                    .Include(l => l.User)
                    .Include(l => l.Workout)
                    .OrderByDescending(l => l.CompletedDate)
                    .Take(5)
                    .ToListAsync()
            };
            return View(stats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
