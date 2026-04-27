using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Data;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string workoutType)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentType"] = workoutType;

            var workoutsList = await _context.Workouts.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                workoutsList = workoutsList.Where(w => w.Name.ToLower().Contains(lowerSearch)).ToList();
            }

            if (!string.IsNullOrEmpty(workoutType))
            {
                workoutsList = workoutsList.Where(w => w.WorkoutType == workoutType).ToList();
            }

            return View(workoutsList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var workout = await _context.Workouts.FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null) return NotFound();

            return View(workout);
        }

        public IActionResult Create()
        {
            return View(new Workout());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,WorkoutType,Notes,MetValue")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workout = await _context.Workouts.FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null) return NotFound();

            return View(workout);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
