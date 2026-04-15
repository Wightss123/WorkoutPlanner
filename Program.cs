using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using WorkoutPlanner.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllersWithViews();

var app = builder.Build();

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WorkoutPlanner.Data.ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    if (!System.Linq.Enumerable.Any(context.Workouts))
    {
        context.Workouts.AddRange(
            new WorkoutPlanner.Models.Workout { Name = "Біг на доріжці", WorkoutType = "Cardio", MetValue = 8.0, Notes = "Біг з рівномірним темпом" },
            new WorkoutPlanner.Models.Workout { Name = "Берпі", WorkoutType = "Interval", MetValue = 8.0, Notes = "Інтенсивна інтервальна вправа" },
            new WorkoutPlanner.Models.Workout { Name = "Жим лежачи", WorkoutType = "Strength", MetValue = 6.0, Notes = "Базова силова вправа на груди" },
            new WorkoutPlanner.Models.Workout { Name = "Планка", WorkoutType = "Strength", MetValue = 4.0, Notes = "Статична вправа на кор" },
            new WorkoutPlanner.Models.Workout { Name = "Стрибки на скакалці", WorkoutType = "Interval", MetValue = 10.0, Notes = "Кардіо розминка" },
            new WorkoutPlanner.Models.Workout { Name = "Розтяжка", WorkoutType = "Flexibility", MetValue = 2.5, Notes = "Загальна розтяжка після тренування" }
        );
        context.SaveChanges();
    }

    if (!System.Linq.Enumerable.Any(context.Users))
    {
        context.Users.AddRange(
            new WorkoutPlanner.Models.User { FirstName = "Олексій", LastName = "Шевченко", Age = 28, WeightKg = 85.5, Goal = "Схуднення / Жироспалювання" },
            new WorkoutPlanner.Models.User { FirstName = "Марія", LastName = "Коваленко", Age = 24, WeightKg = 54.0, Goal = "Набір м'язової маси" },
            new WorkoutPlanner.Models.User { FirstName = "Іван", LastName = "Франко", Age = 35, WeightKg = 78.0, Goal = "Підтримка форми / Тонус" },
            new WorkoutPlanner.Models.User { FirstName = "Олена", LastName = "Бойко", Age = 42, WeightKg = 65.0, Goal = "Відновлення / Реабілітація" },
            new WorkoutPlanner.Models.User { FirstName = "Андрій", LastName = "Мельник", Age = 19, WeightKg = 70.0, Goal = "Рельєф / Сушка" }
        );
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
