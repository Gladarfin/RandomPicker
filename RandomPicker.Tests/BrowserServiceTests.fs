module RandomPicker.Tests.BrowserServiceTests

open System
open RandomPicker.App.Services
open Xunit
open Foq

[<Fact>]
let ``OpenUrlInDefaultBrowser should open default browser with provided url`` () =
    let mockLauncher =
        Mock<IBrowserLauncher>()
            .Setup(fun x -> <@ x.OpenUrlInBrowser(any()) @>)
            .Calls<string>(fun _ -> ())
            .Create()
    let service = BrowserService(mockLauncher)
    let testUrl = "youtube.com"
        
    service.OpenUrlInDefaultBrowser(testUrl)
    verify <@ mockLauncher.OpenUrlInBrowser(testUrl) @> once
        
[<Fact>]
let ``OpenUrlInDefaultBrowser should rethrow exceptions from launcher`` () =
    let expectedEx = Exception("Test error")
    Mock<IBrowserLauncher>()
        .Setup(fun x -> <@ x.OpenUrlInBrowser(any()) @>)
        .Raises(expectedEx)
        .Create()
    |> BrowserService
    |> fun service -> fun () -> service.OpenUrlInDefaultBrowser("https://example.com")
    |> Assert.Throws<Exception>