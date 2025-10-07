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
        Assert.Equal(25, profile.Age)
        Assert.Equal(70.0, profile.Weight)
        Assert.Equal(180.0, profile.Height)
    | _ -> Assert.True(false, "Expected success for valid input")


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



type DomainGenerators =
    static member NonEmptyString() =
        Arb.generate<string>
        |> Gen.where (fun s -> not (String.IsNullOrWhiteSpace(s)))
        |> Gen.map (NonEmptyString.create >> Option.get)
        |> Arb.fromGen

    static member EmailAddress() =
        Arb.generate<string>
        |> Gen.where (fun s -> s.Contains("@") && s.Length <= 254)
        |> Gen.map (EmailAddress.create >> Option.get)
        |> Arb.fromGen

    static member PositiveInt() =
        Gen.choose(1, 10000)
        |> Gen.map (PositiveInt.create >> Option.get)
        |> Arb.fromGen

    static member FitnessLevel() =
        Arb.from<FitnessLevel>

    static member WorkoutIntensity() =
        Arb.from<WorkoutIntensity>

let registerGenerators() = 
    Arb.register<DomainGenerators>() |> ignore


module PropertyTests =
    do registerGenerators()

    [<Property(Arbitrary = [| typeof<DomainGenerators> |])>]
    let ``UserProfile should always have valid email`` (name: NonEmptyString) (email: EmailAddress) =
        let result = createUserProfile (NonEmptyString.value name) (EmailAddress.value email) None None None None
        
        match result with
        | UserCreated user ->
            EmailAddress.value user.Email |> should equal (EmailAddress.value email)
            user.Email |> EmailAddress.value |> should contain "@"
            
        | InvalidUserData _ -> failwith "Should not fail with valid input"


    [<Property>]
    let ``Progress percentage should be between 0 and 100 for active goals`` 
        (currentValue: float) 
        (targetValue: float) =
        
        // Only test with valid values
        if targetValue > 0.0 && currentValue >= 0.0 then
            let goal = {
                UserId = UserId "test-user"
                GoalType = DailySteps (createPositiveInt 10000)
                TargetValue = targetValue
                CurrentValue = currentValue
                Deadline = None
                Status = Active
                CreatedAt = DateTime.Now
            }
            
            let percentage = GoalExtensions.calculateProgressPercentage goal
            percentage |> should be (greaterThanOrEqualTo 0.0)
            percentage |> should be (lessThanOrEqualTo 100.0)