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
    private Bitmap _thumbnail;
    private static Settings _appSettings;

    public YoutubeApiService(SettingsService settingsService)
    {
        Task.Run(async () =>
        {
            _appSettings = await settingsService.LoadSettingsAsync();
        }).Wait();
    }
    
    public async Task<List<string>> CreateListOfAllVideosFromPlaylists(List<string> playlists)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = _appSettings.ApiKey,
            ApplicationName = _appSettings.ApplicationName
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