using System.IO;
using Newtonsoft.Json;
using RandomPicker.App.Models;

namespace RandomPicker.App.Services;

public class LoadSettings
{
    public static Settings Load(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("File not found, check path to file", filePath);
        
        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Settings>(json);
    }
}