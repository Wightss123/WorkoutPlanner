namespace WorkoutPlanner.Models
{
    public class FlexibilityWorkoutLog : WorkoutLog
    {

        public override string GetDetails()
        {
            return $"Тривалість: {DurationMinutes} хв";
        }
    }
}
