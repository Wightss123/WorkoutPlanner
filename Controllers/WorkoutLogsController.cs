using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Data;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Controllers
{
    public class WorkoutLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? clientId, string sortOrder)
        {
            var logs = _context.WorkoutLogs.Include(w => w.User).Include(w => w.Workout).AsQueryable();
            if (clientId.HasValue)
            {
                logs = logs.Where(w => w.UserId == clientId.Value);
            }
            
            ViewData["Clients"] = new SelectList(_context.Users, "Id", "FullName", clientId);
            ViewData["CurrentClient"] = clientId;
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "date_desc";
            }

            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSort"] = sortOrder == "date_desc" ? "date_asc" : "date_desc";
            ViewData["ClientSort"] = sortOrder == "client_asc" ? "client_desc" : "client_asc";
            ViewData["TypeSort"] = sortOrder == "type_asc" ? "type_desc" : "type_asc";

            var logsList = await logs.ToListAsync();

            logsList = sortOrder switch
            {
                "date_asc" => logsList.OrderBy(l => l.CompletedDate).ThenBy(l => l.Id).ToList(),
                "client_asc" => logsList.OrderBy(l => l.User?.LastName).ThenByDescending(l => l.CompletedDate).ToList(),
                "client_desc" => logsList.OrderByDescending(l => l.User?.LastName).ThenByDescending(l => l.CompletedDate).ToList(),
                "type_asc" => logsList.OrderBy(l => l.WorkoutType).ThenByDescending(l => l.CompletedDate).ToList(),
                "type_desc" => logsList.OrderByDescending(l => l.WorkoutType).ThenByDescending(l => l.CompletedDate).ToList(),
                "date_desc" => logsList.OrderByDescending(l => l.CompletedDate).ThenByDescending(l => l.Id).ToList(),
                _ => logsList.OrderByDescending(l => l.CompletedDate).ThenByDescending(l => l.Id).ToList()
            };

            return View(logsList);
        }

        public IActionResult Create(int? userId)
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", userId);
            var workouts = _context.Workouts.OrderBy(w => w.WorkoutType).ThenBy(w => w.Name).ToList();
            ViewData["WorkoutsJson"] = System.Text.Json.JsonSerializer.Serialize(workouts.Select(w => new { w.Id, w.Name, w.WorkoutType }));
            
            var selectListItems = new System.Collections.Generic.List<SelectListItem>();
            foreach (var group in workouts.GroupBy(w => w.WorkoutType))
            {
                var selectGroup = new SelectListGroup { Name = group.Key switch { "Cardio" => "Кардіо", "Strength" => "Силове", "Interval" => "Інтервальне", _ => "Гнучкість" } };
                foreach (var workout in group)
                {
                    selectListItems.Add(new SelectListItem { Value = workout.Id.ToString(), Text = workout.Name, Group = selectGroup });
                }
            }
            ViewData["WorkoutId"] = selectListItems;
            
            return View(new WorkoutLogViewModel { UserId = userId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutLogViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var workout = await _context.Workouts.FindAsync(vm.WorkoutId);
                if (workout == null) return NotFound();

                WorkoutLog log;
                switch (workout.WorkoutType)
                {
                    case "Cardio":
                        log = new CardioWorkoutLog
                        {
                            AverageHeartRate = vm.AverageHeartRate ?? 120,
                            DistanceKm = vm.DistanceKm ?? 0
                        };
                        break;
                    case "Strength":
                        log = new StrengthWorkoutLog
                        {
                            WeightLiftedKg = vm.WeightLiftedKg ?? 0,
                            Sets = vm.Sets ?? 1,
                            Reps = vm.Reps ?? 1
                        };
                        break;
                    case "Interval":
                        log = new IntervalWorkoutLog
                        {
                            Sets = vm.Sets ?? 1,
                            Reps = vm.Reps ?? 10,
                            WorkDurationSeconds = vm.WorkDurationSeconds ?? 40,
                            RestDurationSeconds = vm.RestDurationSeconds ?? 20
                        };
                        break;
                    case "Flexibility":
                    default:
                        log = new FlexibilityWorkoutLog();
                        break;
                }

                log.UserId = vm.UserId;
                log.WorkoutId = vm.WorkoutId;
                log.WorkoutType = workout.WorkoutType;
                log.CompletedDate = vm.CompletedDate;
                log.DurationMinutes = vm.DurationMinutes;
                log.Comment = vm.Comment;

                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Users", new { id = vm.UserId });
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", vm.UserId);
            var workoutsList = _context.Workouts.Select(w => new { w.Id, w.Name, w.WorkoutType }).ToList();
            ViewData["WorkoutsJson"] = System.Text.Json.JsonSerializer.Serialize(workoutsList);
            ViewData["WorkoutId"] = new SelectList(workoutsList, "Id", "Name", vm.WorkoutId);
            return View(vm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workoutLog = await _context.WorkoutLogs
                .Include(w => w.User)
                .Include(w => w.Workout)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (workoutLog == null) return NotFound();

            return View(workoutLog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutLog = await _context.WorkoutLogs.FindAsync(id);
            if (workoutLog != null)
            {
                _context.WorkoutLogs.Remove(workoutLog);
                await _context.SaveChangesAsync();
            }
            
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
