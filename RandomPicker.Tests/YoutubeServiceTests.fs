module RandomPicker.Tests.YoutubeServiceTests

open System.Collections.Generic
open Foq
open Xunit
open RandomPicker.App.Services
open RandomPicker.App.Models
open Google.Apis.YouTube.v3.Data

let createMockYoutubeServiceResponse(videoIds: string list)(nextPageToken:string option) =
    let response = PlaylistItemListResponse()
    response.Items <- List(videoIds |> List.map (fun id ->
        PlaylistItem(
            Snippet = PlaylistItemSnippet(
                ResourceId = ResourceId(VideoId = id)
            )
        )
    ))
    response.NextPageToken <- nextPageToken |> Option.toObj
    response
let settingsForTests = Settings(ApiKey = "test-api-key", ApplicationName = "test-app-name")    
let mockSettingsService = 
    Mock<SettingsService>()
        .Setup(fun x -> <@ x.LoadSettings() @>)
        .Returns(settingsForTests)
        .Create()

[<Fact>]
let ``CreateListOfAllVideosFromPlaylists should return an empty list for empty input`` () =
    async {
        let service = YoutubeApiService(mockSettingsService)
        let! result = service.CreateListOfAllVideosFromPlaylists(List<string>()) |> Async.AwaitTask
        Assert.Empty(result)
    }

module MockTests =
    [<Fact>]
    let ``Verify mock is actually being used`` () =
        async {
            let mutable wasCalled = false
            let mockSettingsService = 
                Mock<SettingsService>()
                    .Setup(fun x -> <@ x.LoadSettings() @>)
                    .Returns(fun () -> 
                        wasCalled <- true
                        Settings(ApiKey = "test-key"))
                    .Create()

            let service = YoutubeApiService(mockSettingsService)
            Assert.True(wasCalled, "The mock was never called!")
        }
        
    [<Fact>]
    let ``Check if settings are being modified`` () =
        let originalSettings = Settings(ApiKey = "test-api-key")
        let loadedSettings = mockSettingsService.LoadSettings()
        Assert.Equal(originalSettings.ApiKey, loadedSettings.ApiKey)
        
    [<Fact>]
    let ``Direct mock verification`` () =
        let testSettings = Settings(ApiKey = "test-key")
        
        let mock = 
            Mock<SettingsService>()
                .Setup(fun x -> <@ x.LoadSettings() @>)
                .Returns(testSettings)
                .Create()

        let loadedSettings = mock.LoadSettings()
        Assert.Equal("test-key", loadedSettings.ApiKey)