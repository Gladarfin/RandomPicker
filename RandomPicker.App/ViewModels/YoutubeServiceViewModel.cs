using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;
using RandomPicker.App.Models;
using RandomPicker.App.Services;
using ReactiveUI;

namespace RandomPicker.App.ViewModels;

public class YoutubeServiceViewModel : ReactiveObject
{
    private UrlsModel _urls;
    private List<string> _playlists;
    private List<string> _videos;
    private int _randomNumber;
    
    public YoutubeServiceViewModel()
    {
        FetchVideosCommand = ReactiveCommand.CreateFromTask(FetchVideoAsync);
        var pathToFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\Urls.json"));
        if (!File.Exists(pathToFile))
        {
            //Add dialog-box here
            
            return;
        }
        DeserializeUrls(pathToFile);
        //preload videos from playlists
        Task.Run(FetchVideoAsync);
        MessageBus.Current.Listen<RandomNumberMessage>().Subscribe(message => _randomNumber = message.RandomNumber);
    }
    
    public ReactiveCommand<Unit, Unit> FetchVideosCommand { get; }

    private void DeserializeUrls(string pathToFile)
    {
        _urls = JsonSerializer.Deserialize<UrlsModel>(File.ReadAllText(pathToFile))!;
        _playlists = _urls.Playlists;
    }

    private async Task FetchVideoAsync()
    {
        var youtubeService = new YoutubeApiService();
        _videos = await youtubeService.CreateListOfAllVideosFromPlaylists(_playlists);
        MessageBus.Current.SendMessage(new VideoCountMessage(_videos.Count));
    }
}