using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public  class SettingsService(string settingsFilePath)
{
    public virtual Settings LoadSettings()
    {
        if (!File.Exists(settingsFilePath))
        {
            return new Settings();
        }
        var json = File.ReadAllText(settingsFilePath);
        return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
    }
}