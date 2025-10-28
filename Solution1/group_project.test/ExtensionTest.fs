module group_project.Test.ExtensionTest

open System
open Xunit
open FsCheck
open group_project.DomainTypes
open group_project.Functions
open group_project.Extension
open FsCheck.Xunit

//Classic Unit tests
//1.
[<Fact>]
let ``CreateUserProfile should return error if name or email is missing`` () =
    let result = validUserProfileCreation "" "" (Some 25) (Some 70.0) (Some 180.0) (Some Beginner)
    match result with
    | Error msg -> Assert.Contains("Name and email", msg)
    | _ -> Assert.True(false, "Expected an error for missing name/email")

[<Fact>]
let ``CreateUserProfile should return success for valid input`` () =
    let result = validUserProfileCreation "Alice" "alice@example.com" (Some 25) (Some 70.0) (Some 180.0) (Some Beginner)
    match result with
    | Success profile ->
        Assert.Equal("Alice", profile.Name)
        Assert.Equal("alice@example.com", profile.Email)
    | _ -> Assert.True(false, "Expected success for valid input")

//3.
[<Fact>]
let ``setFitnessGoal should mark goal as completed if past deadline and complete`` () =
    let expiredGoal =
        setFitnessGoal (UserId "u1") (Distance 10.0) 10.0 (Some 10.0) (Some (DateTime.Now.AddDays(-3.0)))
    Assert.Equal(GoalStatus.Completed, expiredGoal.Status)

[<Fact>]
let ``setFitnessGoal should create active goal if future deadline`` () =
    let goal =
        setFitnessGoal (UserId "u1") (Distance 10.0) 10.0 None (Some (DateTime.Now.AddDays(7.0)))
    Assert.Equal(GoalStatus.Active, goal.Status)

//5.
[<Fact>]
let ``updateGoalProgress should mark goal as completed when reaching target`` () =
    let goal =
        { UserId = UserId("u1")
          GoalType = Distance 5.0
          TargetValue = 5.0
          CurrentValue = 3.0
          Deadline = (Some (DateTime.Now.AddDays(2.0)))
          Status = Active
          CreatedAt = DateTime.Now }

    let updated = updateGoalProgress goal 2.5
    Assert.Equal(GoalStatus.Completed, updated.Status)


//6.
[<Fact>]
let ``suggestWorkouts should return Superior workouts for superior fitness level`` () =
    let user =
        { Id = UserId("u1")
          Name = "John"
          Email = "john@example.com"
          Age = Some 28
          Weight = Some 75.0
          Height = Some 180.0
          FitnessLevel = Some Expert
          NotificationPreferences = Both
          CreatedAt = DateTime.Now }

    let workouts = suggestWorkouts user []
    Assert.Contains(Running(Some 30.0), workouts)


//7.
[<Fact>]
let ``calculateProgressWithMessage should include motivational message`` () =
    let goals: FitnessGoal list =
        [{ UserId = UserId("u1")
           GoalType = Distance 10.0
           TargetValue = 10.0
           CurrentValue = 0.0
           Deadline = (Some (DateTime.Now.AddDays(7.0)))
           Status = Active
           CreatedAt = DateTime.Now }]
        
    let metricSelector (entry: WorkoutEntry) = entry.WorkoutType

    let entries: float list = [1.0; 2.0; 3.0; 4.0]
    let result = calculateProgressWithMotivation goals entries id
    let (updatedGoal, message) = List.head result
    Assert.NotEmpty(message)
    Assert.Contains("Goal", message)


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

    let (feedbackString, feedbackNr) = provideFeedback workout
    Assert.Contains("Great", feedbackString)
    Assert.Equal(10, feedbackNr)

//16.
[<Fact>]
let ``scheduleGroupWorkout should set date`` () =
    let participants = [UserId("u1"); UserId("u2")]
    let date = DateTime.Now.AddDays(5.0)
    let workoutType = Running(Some 5.0)

    let scheduled = scheduleGroupWorkout participants workoutType date
    Assert.Equal(date, scheduled.Date)


//Property based Test with
[<Property>]
let ``calculateProgressWithMotivation returns correct progress and messages`` (goals: FitnessGoal list) (entries: WorkoutEntry list) =
    let metricSelector (w: WorkoutEntry) = float w.Duration
    let results = calculateProgressWithMotivation goals entries metricSelector
    // Property 1: Die Länge der Ergebnisliste entspricht der Länge der Goals-Liste
    let lengthOk = List.length results = List.length goals
    // Property 2: CurrentValue jedes Goals entspricht der Summe der metricSelector-Werte
    let valuesOk =
        List.forall2 (fun goal (updatedGoal, _) ->
            let expected = entries |> List.sumBy metricSelector
            updatedGoal.CurrentValue = expected
        ) goals results
    lengthOk && valuesOk
