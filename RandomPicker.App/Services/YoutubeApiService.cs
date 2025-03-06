using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public class YoutubeApiService
{
    private static Settings _appSettings = new();

    public YoutubeApiService()
    {
        var pathToFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\Settings.json"));
        _appSettings = LoadSettings.Load(pathToFile);
    }
    
    public async Task<List<string>> CreateListOfAllVideosFromPlaylists(List<string> playlists)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = _appSettings.ApiKey,
            ApplicationName = _appSettings.ApplicationName
        });

        var videos = new List<string>();

        foreach (var list in playlists)
        {
            var playlistItemRequest = youtubeService.PlaylistItems.List("snippet");
            playlistItemRequest.PlaylistId = list;
            playlistItemRequest.MaxResults = 50;

            PlaylistItemListResponse response;
            do
            {
                response = await playlistItemRequest.ExecuteAsync();

                videos.AddRange(response.Items.Select(item => item.Snippet.ResourceId.VideoId));

                playlistItemRequest.PageToken = response.NextPageToken;
            } while (!string.IsNullOrEmpty(response.NextPageToken));
        }

        return videos;
    }
}