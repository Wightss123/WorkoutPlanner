using System.ComponentModel.DataAnnotations;

namespace WorkoutPlanner.Models
{
    public class CardioWorkoutLog : WorkoutLog
    {
        [Range(40, 220)]
        public int AverageHeartRate { get; set; }

        [Range(0, 100)]
        public double DistanceKm { get; set; }


        public override string GetDetails()
        {
            return $"Дистанція: {DistanceKm} км, Пульс: {AverageHeartRate} уд/хв";
        }

        public override double CalculateCaloriesBurned()
        {
            double baseCalories = base.CalculateCaloriesBurned();
            if (AverageHeartRate > 140)
            {
                return baseCalories * 1.15;
            }
            return baseCalories;
        }
    }
}
