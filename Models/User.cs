using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutPlanner.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public double? WeightKg { get; set; }
        public string Goal { get; set; } = string.Empty;

        public ICollection<WorkoutLog> WorkoutLogs { get; set; } = new List<WorkoutLog>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
