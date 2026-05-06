using System;

namespace WorkoutPlanner.Models
{
    public class IntervalWorkoutLog : WorkoutLog
    {
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int WorkDurationSeconds { get; set; }
        public int RestDurationSeconds { get; set; }


        public override string GetDetails()
        {
            if (WorkDurationSeconds > 0)
            {
                return $"{Sets} раундів ({WorkDurationSeconds}с робота, {RestDurationSeconds}с відпочинок)";
            }
            return $"{Sets} підходів по {Reps} разів";
        }

        public override double CalculateCaloriesBurned()
        {
            double baseCalories = base.CalculateCaloriesBurned();
            
            if (RestDurationSeconds > 0 && WorkDurationSeconds > RestDurationSeconds)
            {
                return baseCalories * 1.20;
            }
            
            return baseCalories;
        }
    }
}
