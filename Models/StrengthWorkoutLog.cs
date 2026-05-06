using System.ComponentModel.DataAnnotations;

namespace WorkoutPlanner.Models
{
    public class StrengthWorkoutLog : WorkoutLog
    {
        [Range(1, 500)]
        public double WeightLiftedKg { get; set; }

        [Range(1, 50)]
        public int Sets { get; set; }

        [Range(1, 100)]
        public int Reps { get; set; }


        public override string GetDetails()
        {
            return $"Вага: {WeightLiftedKg} кг, Підходів: {Sets}, Повторень: {Reps}";
        }

        public override double CalculateCaloriesBurned()
        {
            double baseCalories = base.CalculateCaloriesBurned();
            double tonnage = WeightLiftedKg * Sets * Reps;
            double extraCalories = tonnage * 0.01;
            
            return baseCalories + extraCalories;
        }
    }
}
