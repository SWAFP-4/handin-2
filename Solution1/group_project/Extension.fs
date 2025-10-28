module group_project.Extension

open System
open group_project.DomainTypes

// 1. Create personal fitness profile
// making the name and email mandatory when creating the profile
type UserProfileCreation = 
    |Success of UserProfile
    |Error of string

let validUserProfileCreation name email age weight height fitnessLevel = 
    if  String.IsNullOrWhiteSpace(name) && String.IsNullOrWhiteSpace(email) then 
        Error "Name and email are mandatory"
    elif String.IsNullOrWhiteSpace(name) then 
        Error "Name is mandatory"
    elif String.IsNullOrWhiteSpace(email) then 
        Error "Email is mandatory"
    else
        let profile = {
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
        Success profile

// 3. Set fitness goals
// updating the status
let updateGoalProgress (goal: FitnessGoal) (progress: float) : FitnessGoal =
    let newValue = goal.CurrentValue + progress
    let newStatus =
        if newValue >= goal.TargetValue then GoalStatus.Completed
        else goal.Status
    { goal with CurrentValue = newValue; Status = newStatus }

// 5. View progress through charts
// user can see motivational message while viewing the their progress
let calculateProgressWithMotivation (goals: FitnessGoal list) (entries: 'a list) metricSelector =
    goals
    |> List.map (fun goal ->
        let progress = entries |> List.sumBy metricSelector
        let updatedGoal = { goal with CurrentValue = progress }

        let message =
            match progress / goal.TargetValue with
            | r when r >= 1.0 -> "Goal achieved!"
            | r when r >= 0.75 -> "Almost there!"
            | r when r >= 0.5 -> "You're halfway!"
            | _ -> "Every step counts!"

        updatedGoal, message
    )

// 6. Personalized workout suggestions
// adding a superior level to the other levels
let suggestWorkouts (user: UserProfile) (workoutHistory: WorkoutEntry list) =
    match user.FitnessLevel with
    | Some FitnessLevel.Beginner -> [Yoga; Walking(Some 5000); Lifting]
    | Some FitnessLevel.Intermediate -> [Running(Some 5.0); HIIT; Swimming(Some 20)]
    | Some FitnessLevel.Advanced -> [Running(Some 10.0); Cycling(Some 20.0); HIIT]
    | Some FitnessLevel.Expert -> [Running(Some 30.0); Cycling(Some 80.0); HIIT]
    | _ -> [Walking(Some 3000); Yoga; Lifting]


// 7. Join fitness challenges
// making the challeneges optional
let joinChallengeOptional (challenge: FitnessChallenge) (userId: UserId option) =
    match userId with
    | Some id -> { challenge with Participants = id :: challenge.Participants }
    | None -> challenge


// 12. Workout performance feedback
// adding a numerical score out of 10
let provideFeedback (workout: WorkoutEntry) =
    match workout.Intensity with
    | High -> ("Great intensity! Keep pushing!", 10)
    | Medium -> ("Good workout. Consider increasing intensity gradually.", 7)
    | Low -> ("Nice active recovery day. Remember to challenge yourself next time.", 4)


// 16. Group workouts
//schedule group workout
type ScheduledGroupWorkout = {
    Participants: UserId list
    WorkoutType: WorkoutType
    Date: DateTime
}

let scheduleGroupWorkout (participants: UserId list) (workoutType: WorkoutType) (date: DateTime) =
    { Participants = participants; WorkoutType = workoutType; Date = date }









