using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutPlanner.Models
{
    public abstract class WorkoutLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int WorkoutId { get; set; }

        [ForeignKey("WorkoutId")]
        public Workout? Workout { get; set; }

        [DataType(DataType.Date)]
        public DateTime CompletedDate { get; set; } = DateTime.Today;

        [Required]
        [Range(1, 300)]
        public int DurationMinutes { get; set; }

        [StringLength(300)]
        public string? Comment { get; set; }

        public string WorkoutType { get; set; } = string.Empty;

        public virtual double CalculateCaloriesBurned()
        {
            double weight = User?.WeightKg ?? 70.0;
            double met = Workout?.MetValue ?? 5.0;
            return met * weight * DurationMinutes * 0.0175;
        }
        
        public abstract string GetDetails();


    }
}
