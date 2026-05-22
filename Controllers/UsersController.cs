using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Data;
using WorkoutPlanner.Models;
using WorkoutPlanner.Services;

namespace WorkoutPlanner.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RecommendationService _recommendationService;

        public UsersController(ApplicationDbContext context, RecommendationService recommendationService)
        {
            _context = context;
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["AgeSort"] = sortOrder == "age_asc" ? "age_desc" : "age_asc";
            ViewData["WeightSort"] = sortOrder == "weight_asc" ? "weight_desc" : "weight_asc";
            ViewData["GoalSort"] = sortOrder == "goal_asc" ? "goal_desc" : "goal_asc";

            var usersList = await _context.Users.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                usersList = usersList.Where(s => s.FirstName.ToLower().Contains(lowerSearch) 
                                      || s.LastName.ToLower().Contains(lowerSearch) 
                                      || (s.Goal != null && s.Goal.ToLower().Contains(lowerSearch))).ToList();
            }

            usersList = sortOrder switch
            {
                "name_asc" => usersList.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList(),
                "name_desc" => usersList.OrderByDescending(u => u.LastName).ThenByDescending(u => u.FirstName).ToList(),
                "age_asc" => usersList.OrderBy(u => u.Age ?? 0).ToList(),
                "age_desc" => usersList.OrderByDescending(u => u.Age ?? 0).ToList(),
                "weight_asc" => usersList.OrderBy(u => u.WeightKg ?? 0).ToList(),
                "weight_desc" => usersList.OrderByDescending(u => u.WeightKg ?? 0).ToList(),
                "goal_asc" => usersList.OrderBy(u => u.Goal).ToList(),
                "goal_desc" => usersList.OrderByDescending(u => u.Goal).ToList(),
                _ => usersList.OrderBy(u => u.LastName).ToList()
            };

            return View(usersList);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.WorkoutLogs)
                .ThenInclude(wl => wl.Workout)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (user == null) return NotFound();

            user.WorkoutLogs = user.WorkoutLogs.OrderByDescending(l => l.CompletedDate).ToList();

            double cardioCals = 0, strengthCals = 0, flexibilityCals = 0, intervalCals = 0;
            
            foreach(var log in user.WorkoutLogs)
            {
                double cals = log.CalculateCaloriesBurned();
                if(log.WorkoutType == "Cardio") cardioCals += cals;
                else if(log.WorkoutType == "Strength") strengthCals += cals;
                else if(log.WorkoutType == "Flexibility") flexibilityCals += cals;
                else if(log.WorkoutType == "Interval") intervalCals += cals;
            }

            ViewBag.CardioCals = Math.Round(cardioCals);
            ViewBag.StrengthCals = Math.Round(strengthCals);
            ViewBag.FlexibilityCals = Math.Round(flexibilityCals);
            ViewBag.IntervalCals = Math.Round(intervalCals);

            ViewBag.Recommendation = _recommendationService.GenerateRecommendation(user, cardioCals, strengthCals, flexibilityCals, intervalCals);

            return View(user);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Age,WeightKg,Goal")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Age,WeightKg,Goal")] User user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ExportToFile()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.WorkoutLogs)
                    .ThenInclude(wl => wl.Workout)
                    .ToListAsync();

                var lines = new System.Text.StringBuilder();
                lines.AppendLine("===============================================================");
                lines.AppendLine("                 FITPLANNER — Звіт по клієнтах");
                lines.AppendLine($"                 Дата формування: {DateTime.Now:dd.MM.yyyy HH:mm}");
                lines.AppendLine("===============================================================");
                lines.AppendLine();

                foreach (var user in users)
                {
                    lines.AppendLine("---------------------------------------------------------------");
                    lines.AppendLine($"  Клієнт: {user.FullName}");
                    lines.AppendLine($"  Вік: {user.Age?.ToString() ?? "Не вказано"} | Вага: {user.WeightKg?.ToString() ?? "Не вказано"} кг");
                    lines.AppendLine($"  Мета: {user.Goal ?? "Не вказано"}");
                    lines.AppendLine($"  Кількість тренувань: {user.WorkoutLogs.Count}");

                    if (user.WorkoutLogs.Any())
                    {
                        double totalCalories = 0;
                        foreach (var log in user.WorkoutLogs)
                        {
                            totalCalories += log.CalculateCaloriesBurned();
                        }
                        lines.AppendLine($"  Загалом калорій: {totalCalories:F0} kcal");
                        lines.AppendLine();
                        lines.AppendLine($"  Журнал тренувань:");
                        foreach (var log in user.WorkoutLogs.OrderByDescending(l => l.CompletedDate))
                        {
                            lines.AppendLine($"    • {log.CompletedDate:dd.MM.yyyy} — {log.Workout?.Name ?? "Невідомо"} ({log.WorkoutType}), {log.DurationMinutes} хв, {log.CalculateCaloriesBurned():F0} kcal");
                        }
                    }
                    lines.AppendLine();
                }

                lines.AppendLine("===============================================================");
                lines.AppendLine($"Всього клієнтів: {users.Count}");

                var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "FitPlanner_Export.txt");
                await System.IO.File.WriteAllTextAsync(tempPath, lines.ToString(), System.Text.Encoding.UTF8);

                var fileBytes = await System.IO.File.ReadAllBytesAsync(tempPath);

                return File(fileBytes, "text/plain; charset=utf-8", $"FitPlanner_Звіт_{DateTime.Now:yyyy-MM-dd}.txt");
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["Error"] = $"Помилка доступу до файлу: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (System.IO.IOException ex)
            {
                TempData["Error"] = $"Помилка запису файлу: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Непередбачена помилка: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
