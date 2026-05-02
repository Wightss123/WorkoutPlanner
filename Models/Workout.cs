using System.ComponentModel.DataAnnotations;

namespace WorkoutPlanner.Models
{
    public class Workout
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'Назва тренування' є обов'язковим.")]
        [StringLength(100)]
        [RegularExpression(@"[\s\S]*[a-zA-Z\u0400-\u04FF][\s\S]*", ErrorMessage = "Назва тренування не може складатись лише з цифр або символів. Додайте літери.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string WorkoutType { get; set; } = string.Empty;

        [Range(1.0, 20.0, ErrorMessage = "MET значення має бути між 1.0 та 20.0")]
        public double MetValue { get; set; } = 5.0;

        [Required(ErrorMessage = "Будь ласка, додайте короткий опис тренування.")]
        [StringLength(500)]
        [RegularExpression(@"[\s\S]*[a-zA-Z\u0400-\u04FF][\s\S]*", ErrorMessage = "Опис тренування не може складатись лише з цифр або символів. Додайте літери.")]
        public string Notes { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} ({WorkoutType})";
        }
    }
}
