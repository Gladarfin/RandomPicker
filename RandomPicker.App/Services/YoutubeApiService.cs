using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public class YoutubeApiService
{
    private readonly List<string> _videos = [];
    private static Settings _appSettings;

    public YoutubeApiService(SettingsService settingsService)
    {
        _appSettings = settingsService.LoadSettings() ?? 
                       throw new ArgumentNullException(nameof(settingsService), "Settings file is missing.");
        ValidateSettings();
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrEmpty(_appSettings.ApiKey))
            throw new ArgumentException("Youtube ApiKey is not configured in Settings.json.");
    }
    
    public async Task<List<string>> CreateListOfAllVideosFromPlaylists(List<string> playlists)
    {
        var youtubeService = InitializeNewYoutubeService();
        
        foreach (var list in playlists)
        {
            await ProcessPlaylist(youtubeService, list);
        }
        return _videos;
    }

    private YouTubeService InitializeNewYoutubeService()
    {
        return new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = _appSettings.ApiKey,
            ApplicationName = _appSettings.ApplicationName
        });
    }

    private async Task ProcessPlaylist(YouTubeService youtubeService, string playlistId)
    {
        var playlistItemRequest = youtubeService.PlaylistItems.List("snippet");
        playlistItemRequest.PlaylistId = playlistId;
        playlistItemRequest.MaxResults = 50;

        PlaylistItemListResponse response;
        do
        {
            response = await playlistItemRequest.ExecuteAsync();
            _videos.AddRange(response.Items.Select(item => item.Snippet.ResourceId.VideoId));
            playlistItemRequest.PageToken = response.NextPageToken;
        } while (!string.IsNullOrEmpty(response.NextPageToken));
    }
}