using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public class YoutubeApiService
{
    private readonly List<string> _videos = [];
    private string _videoUrl = string.Empty;
    private Bitmap _thumbnail;
    private string _pathToFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\Settings.json"));
    private static Settings AppSettings { get; set; }

    public YoutubeApiService()
    {
        //Don't like that I'm using SettingsService in each ViewModel where I need it
        Task.Run(async () =>
        {
            var settingsService = new SettingsService(_pathToFile);
            AppSettings = await settingsService.LoadSettingsAsync();
        }).Wait();
    }
    
    public async Task<List<string>> CreateListOfAllVideosFromPlaylists(List<string> playlists)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = AppSettings.ApiKey,
            ApplicationName = AppSettings.ApplicationName
        });
        
        foreach (var list in playlists)
        {
            var playlistItemRequest = youtubeService.PlaylistItems.List("snippet");
            playlistItemRequest.PlaylistId = list;
            playlistItemRequest.MaxResults = 50;

            PlaylistItemListResponse response;
            do
            {
                response = await playlistItemRequest.ExecuteAsync();
                _videos.AddRange(response.Items.Select(item => item.Snippet.ResourceId.VideoId));
                playlistItemRequest.PageToken = response.NextPageToken;
            } while (!string.IsNullOrEmpty(response.NextPageToken));
        }
        return _videos;
    }

  

    
}