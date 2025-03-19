using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public class CompletedVideosService(string completedVideosFilePath, int currentRandomNumber)
{
    private readonly string _completedVideosFilePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), completedVideosFilePath);
    public void UpdateCompletedVideosList()
    {
        if (!File.Exists(_completedVideosFilePath))
        {
            Task.Run(async () => await ResetListAsync()).Wait();
        }

        var json = JsonConvert.DeserializeObject<CompletedVideos>(File.ReadAllText(_completedVideosFilePath));
        json.CompletedList.Add(currentRandomNumber);
        var updatedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
        Task.Run(async() => await File.WriteAllTextAsync(_completedVideosFilePath, updatedJson)).Wait();
    }
    
    public async Task ResetListAsync()
    {
        var newList = JsonConvert.SerializeObject(new CompletedVideos([]), Formatting.Indented); 
        await File.WriteAllTextAsync(_completedVideosFilePath, newList);
    }
    
}