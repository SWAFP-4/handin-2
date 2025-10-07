module FitnessTracker.Domain

open System

type UserId = UserId of string
type ChallengeId = ChallengeId of Guid
type WorkoutId = WorkoutId of Guid
type MealId = MealId of Guid

// I add this so that the compiler can catch mistakes at compile time, and I don't mix values
[<Measure>] type kg
[<Measure>] type cm
[<Measure>] type ml
[<Measure>] type km

type NonEmptyString = private NonEmptyString of string
module NonEmptyString =
    let create (str: string) =
        if String.IsNullOrWhiteSpace(str) then None
        else Some (NonEmptyString (str.Trim()))
    
    let value (NonEmptyString str) = str

type EmailAddress = private EmailAddress of string
module EmailAddress =
    let create (email: string) =
        if String.IsNullOrWhiteSpace(email) then None
        elif email.Contains("@") && email.Length <= 254 then Some (EmailAddress email)
        else None
    
    let value (EmailAddress email) = email

type Weight = private Weight of float<kg>
module Weight =
    let create (kg: float) =
        if kg > 0.0 && kg <= 300.0 then Some (Weight (kg * 1.0<kg>))
        else None
    
    let value (Weight w) = w

type Height = private Height of float<cm>
module Height =
    let create (cm: float) =
        if cm > 0.0 && cm <= 250.0 then Some (Height (cm * 1.0<cm>))
        else None
    
    let value (Height h) = h

type PositiveInt = private PositiveInt of int
module PositiveInt =
    let create (n: int) = if n > 0 then Some (PositiveInt n) else None
    let value (PositiveInt n) = n

type BoundedString = private BoundedString of string
module BoundedString =
    let create maxLength (str: string) =
        if String.IsNullOrWhiteSpace(str) then None
        elif str.Length <= maxLength then Some (BoundedString str)
        else None
    
    let value (BoundedString str) = str


// User Management Context
type FitnessLevel = Beginner | Intermediate | Advanced | Expert
type NotificationPreference = Email | Push | Both 

type UserProfile = {
    Id: UserId
    Name: string
    Email: string
    Age: int option
    Weight: float option // kg
    Height: float option // cm
    FitnessLevel: FitnessLevel option
    NotificationPreferences: NotificationPreference
    CreatedAt: DateTime
}

// Activity Tracking Context
type WorkoutType =
    | Running of distance: float option
    | Cycling of distance: float option
    | Lifting
    | Yoga
    | Swimming of laps: int option
    | HIIT
    | Walking of steps: int option
    | Custom of string

type WorkoutIntensity = Low | Medium | High

type WorkoutEntry = {
    Id: WorkoutId
    UserId: UserId
    Date: DateTime
    WorkoutType: WorkoutType
    Duration: int // minutes
    CaloriesBurned: option<float>
    Intensity: WorkoutIntensity
    Notes: string option
    HeartRate: int option
}

// Nutrition Context
type MealType = Breakfast | Lunch | Dinner | Snack | Other of string

type NutritionEntry = {
    Id: MealId
    UserId: UserId
    Date: DateTime
    Meal: MealType
    Items: (string * int) list // Food item and calories
    TotalCalories: int
}

type HydrationEntry = {
    UserId: UserId
    Date: DateTime
    WaterIntake: int // ml
    Target: int
}

// Goals & Analytics Context
type GoalType = 
    | DailySteps of target: int
    | WeeklyWorkouts of frequency: int
    | WeightLoss of target: float
    | CalorieIntake of target: int
    | Distance of target: float
    | CustomGoal of string

type GoalStatus = Active | Completed | Failed

type FitnessGoal = {
    UserId: UserId
    GoalType: GoalType
    TargetValue: float
    CurrentValue: float
    Deadline: DateTime option
    Status: GoalStatus
    CreatedAt: DateTime
}

type ProgressData = {
    Date: DateTime
    Value: float
    Metric: string
}

// Social Context
type ChallengeStatus = NotStarted | InProgress | Completed

type FitnessChallenge = {
    Id: ChallengeId
    Name: string
    Description: string
    StartDate: DateTime
    EndDate: DateTime
    Target: float
    Metric: string
    Participants: UserId list
}

type Achievement = {
    UserId: UserId
    Title: string
    Description: string
    EarnedDate: DateTime
    SharedOnSocialMedia: bool
}

// Wellness Context
type SleepQuality = Poor | Fair | Good | Excellent

type SleepEntry = {
    UserId: UserId
    Date: DateTime
    Duration: float // hours
    Quality: SleepQuality
    BedTime: DateTime
    WakeTime: DateTime
}

// System Context
type DeviceType = AppleWatch | Fitbit | Garmin | Android | IOS

type DeviceIntegration = {
    UserId: UserId
    DeviceType: DeviceType
    Connected: bool
    LastSync: DateTime option
}

type Notification = {
    UserId: UserId
    Message: string
    SentAt: DateTime
    Read: bool
    Type: string
}

// Workout Library Context
type ExerciseCategory = Cardio | Strength | Flexibility | Balance

type Exercise = {
    Name: string
    Category: ExerciseCategory
    Difficulty: FitnessLevel
    Duration: int
    Calories: int
    VideoUrl: string option
    Description: string
}

type WorkoutPlan = {
    Name: string
    Exercises: Exercise list
    TotalDuration: int
    TotalCalories: int
    RecommendedFor: FitnessLevel list
}