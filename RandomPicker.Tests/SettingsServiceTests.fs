module RandomPicker.Tests.SettingsServiceTests

open System
open System.IO
open RandomPicker.App.Models
open RandomPicker.App.Services
open Xunit
open Newtonsoft.Json

let cleanup path =
        if File.Exists(path) then
            try File.Delete(path) with _ -> ()

[<Fact>]
let ``LoadSetting should returns default Settings when file doesn't exists`` () = task {
    let result =
        Path.Combine(Path.GetTempPath(), "NonExistentSettingsFile.json")
        |> SettingsService
        |> _.LoadSettings()
   
    Assert.Multiple( fun () ->
        result.OpenFileAfterExit |> Assert.True
        result.RandomWithoutRepetitions |> Assert.True
        ("", result.PathToFileWithUrls) |> Assert.Equal
        ("", result.PathToFileWithCompleted) |> Assert.Equal
        ("", result.ApiKey) |> Assert.Equal
        ("", result.ApplicationName) |> Assert.Equal
        (5, result.MaxRandomNumberRerolls) |> Assert.Equal
    )
}

[<Fact>]
let ``LoadSettingsAsync should returns deserialized settings when file is exists`` () = task {
    let testSettings = Settings(
        PathToFileWithCompleted = @"C:\temp\CompletedList.json",
        PathToFileWithUrls = @"C:\temp\Urls.json",
        ApplicationName = "SomeTestName",
        MaxRandomNumberRerolls = 20           
        )
    let tempTestFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json")
    do! File.WriteAllTextAsync(tempTestFile, JsonConvert.SerializeObject(testSettings))
    let result =
        SettingsService(tempTestFile)
        |> _.LoadSettings()
    
    Assert.Multiple(fun () -> 
        ("", result.PathToFileWithCompleted) |> Assert.NotEqual<string>
        ("", result.PathToFileWithUrls) |> Assert.NotEqual<string>
        ("", result.ApplicationName) |> Assert.NotEqual<string>
        (5, result.MaxRandomNumberRerolls) |> Assert.NotEqual
    )
    cleanup tempTestFile
}