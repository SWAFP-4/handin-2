module group_project.Parsing

open System
open group_project.DomainTypes

let parseMealType (mealStr: string) =
    match mealStr.Trim().ToLower() with
    | "breakfast" -> Breakfast
    | "lunch" -> Lunch
    | "dinner" -> Dinner
    | "snack" -> Snack
    | other -> Other(other)

let parseCalorieEntry (line: string) =
    let parts = line.Split([|':'|], 2)
    if parts.Length = 2 then
        let foodItem = parts.[0].Trim()
        let caloriePart = parts.[1].Trim().Replace(" kcal", "").Trim()
        match Int32.TryParse(caloriePart) with
        | (true, calories) -> Some (foodItem, calories)
        | _ -> None
    else
        None

let parseNutritionData (input: string) =
    let lines = input.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
    
    let rec parseLines 
        (lines: string list)
        (currentUser: string option)
        (currentDate: DateTime option)
        (currentMeal: MealType option)
        (items: (string * int) list)
        (entries: NutritionEntry list)
        : NutritionEntry list =
        match lines with
        | [] -> 
            match currentUser, currentDate, currentMeal, items with
            | Some user, Some date, Some meal, item when not items.IsEmpty ->
                let totalCalories = items |> List.sumBy snd
                let entry = {
                    Id = MealId(Guid.NewGuid())
                    UserId = UserId user
                    Date = date
                    Meal = meal
                    Items = items
                    TotalCalories = totalCalories
                }
                entry :: entries
            | _ -> entries
            |> List.rev
            
        | line :: rest ->
            let trimmedLine = line.Trim()
            
            if trimmedLine.StartsWith("User:") then
                let user = trimmedLine.Replace("User:", "").Trim()
                parseLines rest (Some user) currentDate currentMeal [] entries
                
            elif trimmedLine.StartsWith("Date:") then
                let dateStr = trimmedLine.Replace("Date:", "").Trim()
                match DateTime.TryParse(dateStr) with
                | (true, date) -> parseLines rest currentUser (Some date) currentMeal items entries
                | _ -> parseLines rest currentUser currentDate currentMeal items entries
                
            elif trimmedLine.StartsWith("Meal:") then
                let newEntries = 
                    match currentUser, currentDate, currentMeal, items with
                    | Some user, Some date, Some meal, items when not items.IsEmpty ->
                        let totalCalories = items |> List.sumBy snd
                        let entry = {
                            Id = MealId(Guid.NewGuid())
                            UserId = UserId user
                            Date = date
                            Meal = meal
                            Items = items
                            TotalCalories = totalCalories
                        }
                        entry :: entries
                    | _ -> entries
                
                let mealType = trimmedLine.Replace("Meal:", "").Trim() |> parseMealType
                parseLines rest currentUser currentDate (Some mealType) [] newEntries
                
            elif trimmedLine.StartsWith("Total Calories:") then
                parseLines rest currentUser currentDate currentMeal items entries
                
            elif trimmedLine.StartsWith("-") && trimmedLine.Contains(":") then
                match parseCalorieEntry(trimmedLine.TrimStart([|'-'|])) with
                | Some item -> parseLines rest currentUser currentDate currentMeal (item :: items) entries
                | None -> parseLines rest currentUser currentDate currentMeal items entries
                
            elif trimmedLine.StartsWith("------------------------") then
                parseLines rest currentUser currentDate currentMeal items entries
                
            else
                parseLines rest currentUser currentDate currentMeal items entries
    
    parseLines (Array.toList lines) None None None [] []