module group_project.DomainTypes

open System

type UserId = UserId of string
type ChallengeId = ChallengeId of Guid
type WorkoutId = WorkoutId of Guid
type MealId = MealId of Guid

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