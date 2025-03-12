using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public class SettingsService(string settingsFilePath)
{
    public async Task<Settings> LoadSettingsAsync()
    {
        if (!File.Exists(settingsFilePath))
        {
            return new Settings();
        }
        var json = await File.ReadAllTextAsync(settingsFilePath);
        return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
    }

    public async Task SaveSettingsAsync(Settings settings)
    {
        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        await File.WriteAllTextAsync(settingsFilePath, json);
    }
    
}