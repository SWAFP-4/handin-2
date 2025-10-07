module FitnessTracker.Tests.ExtensionTests

open System
open Xunit
open FitnessTracker.Domain
open FitnessTracker.Services
open FitnessTracker.Extension2 

//1.
[<Fact>]
let ``CreateUserProfile should return error if name or email is missing`` () =
    let result = validateUserProfileCreation "" "" 25 70.0 180.0 (Some Beginner)
    match result with
    | Error msg -> Assert.Contains("Name and email", msg)
    | _ -> Assert.True(false, "Expected an error for missing name/email")

[<Fact>]
let ``CreateUserProfile should return success for valid input`` () =
    let result = validateUserProfileCreation "Alice" "alice@example.com" 25 70.0 180.0 (Some Beginner)
    match result with
    | Success profile ->
        Assert.Equal("Alice", profile.Name)
        Assert.Equal("alice@example.com", profile.Email)
    | _ -> Assert.True(false, "Expected success for valid input")

//3.
[<Fact>]
let ``setFitnessGoalWithDeadline should mark goal as expired if past deadline`` () =
    let expiredGoal =
        setFitnessGoalWithDeadline
            (UserId "u1")
            Distance
            10.0
            (DateTime.Now.AddDays(-3.0))
    Assert.Equal(Expired, expiredGoal.Status)

[<Fact>]
let ``setFitnessGoalWithDeadline should create active goal if future deadline`` () =
    let goal =
        setFitnessGoalWithDeadline
            (UserId "u1")
            Distance
            10.0
            (DateTime.Now.AddDays(7.0))
    Assert.Equal(Active, goal.Status)



//5.
[<Fact>]
let ``updateGoalProgress should mark goal as completed when reaching target`` () =
    let goal =
        { UserId = UserId("u1")
          GoalType = Distance
          TargetValue = 5.0
          CurrentValue = 3.0
          Deadline = DateTime.Now.AddDays(2.0)
          Status = Active
          CreatedAt = DateTime.Now }

    let updated = updateGoalProgress goal 2.5
    Assert.Equal(Completed, updated.Status)


//6.
[<Fact>]
let ``suggestWorkouts should return Superior workouts for superior fitness level`` () =
    let user =
        { Id = UserId("u1")
          Name = "John"
          Email = "john@example.com"
          Age = 28
          Weight = 75.0
          Height = 180.0
          FitnessLevel = Some Superior
          NotificationPreferences = Both
          CreatedAt = DateTime.Now }

    let workouts = suggestWorkouts user []
    Assert.Contains(Running(Some 30.0), workouts)


//7.
[<Fact>]
let ``calculateProgressWithMessage should include motivational message`` () =
    let goals =
        [{ UserId = UserId("u1")
           GoalType = Distance
           TargetValue = 10.0
           CurrentValue = 0.0
           Deadline = DateTime.Now.AddDays(7.0)
           Status = Active
           CreatedAt = DateTime.Now }]

    let entries = [1.0; 2.0; 3.0; 4.0]
    let (updatedGoals, message) = calculateProgressWithMessage goals entries id
    Assert.NotEmpty(message)
    Assert.Contains("progress", message.ToLower())


//12.
[<Fact>]
let ``provideFeedbackWithScore should include score`` () =
    let workout =
        { Id = WorkoutId(Guid.NewGuid())
          UserId = UserId("u1")
          Date = DateTime.Now
          WorkoutType = Running(Some 5.0)
          Duration = 45
          CaloriesBurned = None
          Intensity = High
          Notes = None
          HeartRate = None }

    let feedback = provideFeedbackWithScore workout
    Assert.Contains("Score", feedback)

//16.
[<Fact>]
let ``scheduleGroupWorkout should set date and not notify`` () =
    let participants = [UserId("u1"); UserId("u2")]
    let date = DateTime.Now.AddDays(5.0)
    let workoutType = Running(Some 5.0)

    let scheduled = scheduleGroupWorkout participants workoutType date false
    Assert.Equal(date, scheduled.Date)
    Assert.False(scheduled.NotifyParticipants)
