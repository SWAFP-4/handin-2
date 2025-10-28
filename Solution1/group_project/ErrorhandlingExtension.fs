module group_project.Extension

open System
open group_project.DomainTypes

//Using monadic error-handling
// 1. Create personal fitness profile
// making the name and email mandatory when creating the profile
let validUserProfileCreation name email age weight height fitnessLevel : Result<UserProfile, string> =
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
        Ok profile

//Using monadic error-handling
// 3. Set fitness goals
// updating the status
let updateGoalProgress (goal: FitnessGoal) (progress: float) : Result<Fitness, string> =
    if progress < 0.0 then
        Error "Progress cannot be negative"
    else
        let newValue = goal.CurrentValue + progress
        let newStatus =
            if newValue >= goal.TargetValue then GoalStatus.Completed
            else goal.Status
        Ok { goal with CurrentValue = newValue; Status = newStatus }

//Using type error-handling
// 5. View progress through charts
// user can see motivational message while viewing the their progress

type progressWithMotivation =
    |Updated of FitnessGoal *string
    |Skipped of string
    
let calculateProgressWithMotivation (goals: FitnessGoal list) (entries: 'a list) metricSelector =
    goals
    |> List.map (fun goal ->
        let progress = entries |> List.sumBy metricSelector
        let ratio = progress / goal.TargetValue
        let message =
            match progress / goal.TargetValue with
            | r when r >= 1.0 -> "Goal achieved!"
            | r when r >= 0.75 -> "Almost there!"
            | r when r >= 0.5 -> "You're halfway!"
            | _ -> "Every step counts!"
            
        if ratio < 0.5 then
            Skipped message
        else
            let updatedGoal = { goal with CurrentValue = progress }
            updatedGoal, message
    )

//Using monadic error-handling
// 6. Personalized workout suggestions
// adding a superior level to the other levels
let suggestWorkouts (user: UserProfile) (workoutHistory: WorkoutEntry list) : Result<suggestions, string> = 
    match user.FitnessLevel with
    | Some FitnessLevel.Beginner -> Ok [Yoga; Walking(Some 5000); Lifting]
    | Some FitnessLevel.Intermediate -> Ok [Running(Some 5.0); HIIT; Swimming(Some 20)]
    | Some FitnessLevel.Advanced -> Ok [Running(Some 10.0); Cycling(Some 20.0); HIIT]
    | Some FitnessLevel.Expert -> Ok [Running(Some 30.0); Cycling(Some 80.0); HIIT]
    |None -> Error "Fitness level is not chosen"

//Using type error-handling
// 7. Join fitness challenges
// making the challeneges optional

type challenge = 
    |Success of Fitnesschallenge
    |Failure of string

let joinChallengeOptional (challenge: FitnessChallenge) (userId: UserId option): ChallengeResult =
    match userId with
    | Some id -> Success { challenge with Participants = id :: challenge.Participants }
    | None -> Failure "User ID missing"
    
//Using monadic error-handling
// 12. Workout performance feedback
// adding a numerical score out of 10
let provideFeedback (workout: WorkoutEntry) : Result<Feeback, string> =
    match workout.Intensity with
    | High -> ("Great intensity! Keep pushing!", 10)
    | Medium -> ("Good workout. Consider increasing intensity gradually.", 7)
    | Low -> ("Nice active recovery day. Remember to challenge yourself next time.", 4)
    | _ -> Error "Invalid fitness level"

//Using type error-handling
// 16. Group workouts
//schedule group workout
type ScheduledGroupWorkout = {
    Participants: UserId list
    WorkoutType: WorkoutType
    Date: DateTime
}

let scheduleGroupWorkout (participants: UserId list) (workoutType: WorkoutType) (date: DateTime) : Result<ScheduledGroupWorkout, string> =
    if List.isEmpty participants then
        Error "At least one participant is required"
    elif date > DateTime.now then
        Error "Workout date cannot be in the future"
    else
        Ok { Participants = participants; WorkoutType = workoutType; Date = date }
