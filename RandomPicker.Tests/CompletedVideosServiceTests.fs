module RandomPicker.Tests.CompletedVideosServiceTests

open System
open System.IO
open RandomPicker.App.Models
open RandomPicker.App.Services
open Xunit
open Newtonsoft.Json

type CompletedVideosModel = {CompletedList:int list}

let tempTestFile() = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json")
let cleanup path =
        if File.Exists(path) then
            try File.Delete(path) with _ -> ()
        
module CompletedVideosServiceTests =
    [<Fact>]
    let ``UpdateCompletedVideosList should create file if not exists`` () =
        let tempFile = tempTestFile()
        try
            let service = CompletedVideosService(tempFile)
            service.UpdateCompletedVideosList()
            File.Exists(tempFile) |> Assert.True            
        finally
            cleanup tempFile
               
    [<Fact>]
    let ``ResetListAsync should create file with empty list``() =
        let tempFile = tempTestFile()
        try
            let service = CompletedVideosService(tempFile)
            service.UpdateCompletedVideosList()
            service.ResetListAsync().Wait()
            File.ReadAllText(tempFile)
            |> JsonConvert.DeserializeObject<CompletedVideos>
            |> _.CompletedList
            |> Assert.Empty
        finally
            cleanup tempFile

type CompletedVideosServiceTheories() =
    static member TestNumbers : obj[] seq = 
        let rnd = Random()
        seq {
            for _ in 1..10 do
                yield [| rnd.Next(1, 200) |]
        }  
    
    [<Theory>]
    [<MemberData(nameof CompletedVideosServiceTheories.TestNumbers)>]
    member this. ``UpdateCompletedVideosList adds number to file correctly`` (numberToAdd) =
        let tempFile = tempTestFile()
        try
            let service = CompletedVideosService(tempFile, numberToAdd)
            service.UpdateCompletedVideosList()
            
            File.ReadAllText(tempFile)
            |> JsonConvert.DeserializeObject<CompletedVideos>
            |> _.CompletedList
            |> Seq.last
            |> fun actual -> Assert.Equal(numberToAdd, actual)                     
        finally
            cleanup tempFile