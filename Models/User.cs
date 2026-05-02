using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutPlanner.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'Ім'я' є обов'язковим.")]
        [StringLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів.")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ'\-\s]+$", ErrorMessage = "Ім'я може містити лише літери, пробіли, апострофи та дефіси.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Прізвище' є обов'язковим.")]
        [StringLength(50, ErrorMessage = "Прізвище не може перевищувати 50 символів.")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ'\-\s]+$", ErrorMessage = "Прізвище може містити лише літери, пробіли, апострофи та дефіси.")]
        public string LastName { get; set; } = string.Empty;

        [Range(10, 120, ErrorMessage = "Вік повинен бути від 10 до 120 років.")]
        public int? Age { get; set; }

        [Range(20, 300, ErrorMessage = "Вага повинна бути від 20 до 300 кг.")]
        public double? WeightKg { get; set; }

        [Required(ErrorMessage = "Будь ласка, оберіть спортивну мету.")]
        [StringLength(200)]
        public string Goal { get; set; } = string.Empty;

        public ICollection<WorkoutLog> WorkoutLogs { get; set; } = new List<WorkoutLog>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
