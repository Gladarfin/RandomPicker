using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public class CompletedVideosService(string completedVideosFilePath, int currentRandomNumber)
{
    public void UpdateCompletedVideosList()
    {
        if (!File.Exists(completedVideosFilePath))
        {
            Task.Run(async () => await ResetListAsync()).Wait();
        }

        var json = JsonConvert.DeserializeObject<CompletedVideos>(File.ReadAllText(completedVideosFilePath));
        json.CompletedList.Add(currentRandomNumber);
        var updatedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
        Task.Run(async() => await File.WriteAllTextAsync(completedVideosFilePath, updatedJson)).Wait();
    }
    
    public async Task ResetListAsync()
    {
        var newList = JsonConvert.SerializeObject(new CompletedVideos([]), Formatting.Indented); 
        await File.WriteAllTextAsync(completedVideosFilePath, newList);
    }
    
}