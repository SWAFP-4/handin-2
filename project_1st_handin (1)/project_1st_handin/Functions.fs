module FitnessTracker.Services

open System
open FitnessTracker.Domain

// User Stories Implementation

// 1. Create personal fitness profile
let createUserProfile name email age weight height fitnessLevel =
    {
        Id = UserId(Guid.NewGuid().ToString())
        Name = name
        Email = email
        Age = age
        Weight = weight
        Height = height
        FitnessLevel = fitnessLevel
        NotificationPreferences = Both
        CreatedAt = DateTime.Now
    }

// 2. Log daily workouts
let logWorkout userId workoutType duration intensity notes heartRate =
    {
        Id = WorkoutId(Guid.NewGuid())
        UserId = userId
        Date = DateTime.Now
        WorkoutType = workoutType
        Duration = duration
        CaloriesBurned = None
        Intensity = intensity
        Notes = notes
        HeartRate = heartRate
    }

// 3. Set fitness goals
let setFitnessGoal userId goalType targetValue deadline =
    {
        UserId = userId
        GoalType = goalType
        TargetValue = targetValue
        CurrentValue = 0.0
        Deadline = deadline
        Status = Active
        CreatedAt = DateTime.Now
    }

// 4. Track nutrition (NutritionParser.fs)

// 5. View progress through charts
let calculateProgress (goals: FitnessGoal list) (entries: 'a list) metricSelector =
    goals
    |> List.map (fun goal ->
        let progress = entries |> List.sumBy metricSelector // assuming metricSelector extracts relevant metric
        { goal with CurrentValue = progress } // update current value
    )

// 6. Personalized workout suggestions
let suggestWorkouts (user: UserProfile) (workoutHistory: WorkoutEntry list) =
    match user.FitnessLevel with
    | Some Beginner -> [Yoga; Walking(Some 5000); Lifting]
    | Some Intermediate -> [Running(Some 5.0); HIIT; Swimming(Some 20)]
    | Some Advanced -> [Running(Some 10.0); Cycling(Some 20.0); HIIT]
    | _ -> [Walking(Some 3000); Yoga; Lifting]

// 7. Join fitness challenges
let joinChallenge (challenge: FitnessChallenge) userId =
    { challenge with Participants = userId :: challenge.Participants }

// 8. Daily reminders and motivational messages
let generateMotivationalMessage () =
    let messages = [
        "Keep going! You're doing great!"
        "Every workout counts. Stay consistent!"
        "Your future self will thank you!"
        "Progress, not perfection!"
        "You're stronger than you think!"
        "Keep swimming!" // finding nemo reference
    ]
    messages.[Random().Next(messages.Length)]

// 9. Track hydration
let logWaterIntake userId intake target =
    {
        UserId = userId
        Date = DateTime.Now
        WaterIntake = intake
        Target = target
    }

// 10. Device integration (simplified)
let syncWithDevice userId deviceType =
    {
        UserId = userId
        DeviceType = deviceType
        Connected = true
        LastSync = Some DateTime.Now
    }

// 11. Workout library
let exerciseLibrary = [
    {
        Name = "Morning Yoga Flow"
        Category = Flexibility
        Difficulty = Beginner
        Duration = 30
        Calories = 150
        VideoUrl = Some "https://youtu.be/ZiQh8jA5tVM?feature=shared"
        Description = "Gentle yoga for flexibility"
    }
    {
        Name = "HIIT Cardio Blast"
        Category = Cardio
        Difficulty = Advanced
        Duration = 45
        Calories = 400
        VideoUrl = Some "https://youtu.be/M0uO8X3_tEA?feature=shared"
        Description = "High intensity interval training"
    }
]

// 12. Workout performance feedback
let provideFeedback (workout: WorkoutEntry) =
    match workout.Intensity with
    | High -> "Great intensity! Keep pushing!"
    | Medium -> "Good workout. Consider increasing intensity gradually."
    | Low -> "Nice active recovery day. Remember to challenge yourself next time."

// 13. Export fitness data
let exportWorkouts (workouts: WorkoutEntry list) =
    workouts
    |> List.map (fun w -> sprintf "%s,%s,%f,%A" (w.Date.ToString("yyyy-MM-dd")) (w.WorkoutType.ToString()) (float w.Duration) w.Intensity)
    |> String.concat "\n"

// 14. Customize settings
let updateUserPreferences user newPreferences =
    { user with NotificationPreferences = newPreferences }

// 15. Track sleep patterns
let logSleep userId duration quality bedTime wakeTime =
    {
        UserId = userId
        Date = DateTime.Now.Date
        Duration = duration
        Quality = quality
        BedTime = bedTime
        WakeTime = wakeTime
    }

// 16. Group workouts
let createGroupWorkout participants workoutType =
    participants |> List.map (fun userId -> logWorkout userId workoutType 60 Medium None None)

// 17. Insights (Don't think I quite understand)
let generateInsights (user: UserProfile) (workouts: WorkoutEntry list) =
    let totalWorkouts = workouts.Length
    let avgDuration = workouts |> List.averageBy (fun w -> float w.Duration)
    $"You've completed {totalWorkouts} workouts with average duration {avgDuration} minutes."

// 18. Support and guidance : it does nothing at the moment
let getHelpResources () =
    [
        "FAQ: Common questions"
        "Video tutorials"
        "Contact support"
        "Community forum"
    ]

// 19. Share achievements on social media
let shareAchievement achievement platform =
    match platform with
    | "facebook" -> $"Shared on Facebook: {achievement.Title}"
    | "twitter" -> $"Shared on Twitter: {achievement.Title}"
    | _ -> "Shared on social media"

// 20. Review past activities
let getActivityHistory (entries: 'a list) days =
    let cutoff = DateTime.Now.AddDays(-float days)
    entries |> List.filter (fun e -> 
        match box e with
        | :? WorkoutEntry as w -> w.Date >= cutoff
        | :? NutritionEntry as n -> n.Date >= cutoff
        | _ -> false)