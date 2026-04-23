using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Workout> Workouts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }
        public DbSet<CardioWorkoutLog> CardioWorkoutLogs { get; set; }
        public DbSet<StrengthWorkoutLog> StrengthWorkoutLogs { get; set; }
        public DbSet<FlexibilityWorkoutLog> FlexibilityWorkoutLogs { get; set; }
        public DbSet<IntervalWorkoutLog> IntervalWorkoutLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkoutLog>()
                .HasDiscriminator<string>("WorkoutType")
                .HasValue<CardioWorkoutLog>("Cardio")
                .HasValue<StrengthWorkoutLog>("Strength")
                .HasValue<IntervalWorkoutLog>("Interval")
                .HasValue<FlexibilityWorkoutLog>("Flexibility");

            modelBuilder.Entity<WorkoutLog>()
                .HasOne(wl => wl.User)
                .WithMany(u => u.WorkoutLogs)
                .HasForeignKey(wl => wl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkoutLog>()
                .HasOne(wl => wl.Workout)
                .WithMany()
                .HasForeignKey(wl => wl.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
