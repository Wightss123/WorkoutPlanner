using System.ComponentModel.DataAnnotations;

namespace WorkoutPlanner.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string WorkoutType { get; set; } = string.Empty;
        public double MetValue { get; set; } = 5.0;
        public string Notes { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} ({WorkoutType})";
        }
    }
}
