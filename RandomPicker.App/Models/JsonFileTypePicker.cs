using Avalonia.Platform.Storage;

namespace RandomPicker.App.Models;

public class JsonFileTypePicker
{
    public static FilePickerFileType Json { get; } = new("JSON File")
    {
        Patterns = ["*.json"],
        MimeTypes = ["application/json"]
    };
}