using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutPlanner.Models
{
    public class WorkoutLogViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int WorkoutId { get; set; }

        public string WorkoutType { get; set; } = "Strength";

        [DataType(DataType.Date)]
        public DateTime CompletedDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Тривалість обов'язкова")]
        [Range(1, 300, ErrorMessage = "Від 1 до 300 хв")]
        public int DurationMinutes { get; set; }

        [StringLength(300)]
        public string? Comment { get; set; }

        public int? AverageHeartRate { get; set; }
        public double? DistanceKm { get; set; }

        public double? WeightLiftedKg { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }

        public int? WorkDurationSeconds { get; set; }
        public int? RestDurationSeconds { get; set; }
    }
}
