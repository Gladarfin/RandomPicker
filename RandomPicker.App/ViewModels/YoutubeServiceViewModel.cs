using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using RandomPicker.App.Models;
using RandomPicker.App.Services;
using ReactiveUI;

namespace RandomPicker.App.ViewModels;

public class YoutubeServiceViewModel : INotifyPropertyChanged
{
    //const
    private const string _videoPrefix = "https://www.youtube.com/watch?v=";
    private const string _prefixThumbnail = "https://img.youtube.com/vi";
    //private
    private ListOfUrls _listOfUrls;
    private List<string> _playlists;
    private List<string> _videos = [];
    private int _randomNumber;
    private string _videoUrl;
    private Bitmap _thumbnail;

    //public
    public string VideoUrl
    {
        get => _videoUrl;
        set
        {
            _videoUrl = value;
            OnPropertyChanged(nameof(VideoUrl));
        }
    }

    public Bitmap Thumbnail
    {
        get => _thumbnail;
        set
        {
            _thumbnail = value;
            OnPropertyChanged(nameof(Thumbnail));
        }
    }
    
    public YoutubeServiceViewModel()
    {
        FetchVideosCommand = ReactiveCommand.CreateFromTask(FetchVideoAsync);
    }

    public void CheckAndDeserializeFile(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            MessageBus.Current.SendMessage(new FileNotFoundMessage(pathToFile));
            return;
        }
        DeserializeUrls(pathToFile);
        //preload videos from playlists
        Task.Run(async() => await FetchVideoAsync()).Wait();
        SubscribeToMessages();
    }

    private void SubscribeToMessages()
    {
        MessageBus.Current.Listen<RandomNumberMessage>().Subscribe(message =>
        {
            _randomNumber = message.RandomNumber;
            UpdateCurrentVideo();
            GetVideoPreview(_videos[_randomNumber - 1]);
        });
    }
    
    public ReactiveCommand<Unit, Unit> FetchVideosCommand { get; }
    
    private void DeserializeUrls(string pathToFile)
    {
        _listOfUrls = JsonSerializer.Deserialize<ListOfUrls>(File.ReadAllText(pathToFile))!;
        _playlists = _listOfUrls.Playlists;
    }

    private async Task FetchVideoAsync()
    {
        var youtubeService = new YoutubeApiService();
        _videos = await youtubeService.CreateListOfAllVideosFromPlaylists(_playlists);
        MessageBus.Current.SendMessage(new VideoCountMessage(_videos.Count));
    }
    
    private void UpdateCurrentVideo()
    {
        VideoUrl = string.Concat(_videoPrefix, _videos[_randomNumber - 1]);
        MessageBus.Current.SendMessage(new VideoUrlMessage(VideoUrl));
    }
    
    /// <summary></summary>
    /// <param name="videoId"></param>
    /// <param name="thumbnailType">
    /// default: Default thumbnail (120x90)
    /// mqdefault: Medium quality (320x180)
    /// hqdefault: High quality (480x360)
    /// sddefault: Standard definition (640x480)
    /// maxresdefault: Maximum resolution (1280x720)
    /// </param>
    /// <returns></returns>
    private void GetVideoPreview(string videoId, string thumbnailType = "hqdefault")
    {
        var newVideoThumbnailUrl = $"{_prefixThumbnail}/{videoId}/{thumbnailType}.jpg";
        Task.Run(async() => await GetThumbnailAsync(newVideoThumbnailUrl)).Wait();
    }
    
    private async Task GetThumbnailAsync(string thumbnailUrl)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(thumbnailUrl);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            Thumbnail = new Bitmap(stream);
        }
        //if preview is unavailable (video is private, probably) or there is another exception when we try to get thumbnail 
        //we just should reroll number and try to reload
        catch (Exception)
        {
            MessageBus.Current.SendMessage(new ThumbnailLoadFailedMessage());
        }
        
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}