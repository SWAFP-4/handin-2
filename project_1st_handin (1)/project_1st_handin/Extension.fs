module FitnessTracker.Extension

open System
open FitnessTracker.Domain
open FitnessTracker.Services
open FitnessTracker.Parsing


// 1. Create personal fitness profile
// making the name and email mandatory when creating the profile
type UserProfileCreation = 
    |Sucess of UserProfile
    |Error of string

let VaildUserProfileCreation name email age weight height fitnessLevel = 
    if string.IsNullOrWhiteSpace(name) then 
        Error "Name is mandatory"
    elif string.IsNullOrWhiteSpace(Email) then 
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
        Sucess profile


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
type FitnessLevel = Beginner | Intermediate | Advanced | Superior
let suggestWorkouts (user: UserProfile) (workoutHistory: WorkoutEntry list) =
    match user.FitnessLevel with
    | Some Beginner -> [Yoga; Walking(Some 5000); Lifting]
    | Some Intermediate -> [Running(Some 5.0); HIIT; Swimming(Some 20)]
    | Some Advanced -> [Running(Some 10.0); Cycling(Some 20.0); HIIT]
    | Some Superior -> [Running(Some 30.0); Cycling(Some 80.0); HIIT]
    | _ -> [Walking(Some 3000); Yoga; Lifting]








