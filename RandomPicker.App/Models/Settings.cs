namespace RandomPicker.App.Models;

public class Settings
{
    public bool OpenFileAfterExit { get; set; } = true;
    public bool RandomWithoutRepetitions { get; set; } = true;
    public string PathToFileWithUrls { get; set; } = "";
    public string PathToFileWithCompleted { get; set;} = "";
    public string ApiKey { get; set;} = "";
    public string ApplicationName { get; set;} = "";
    public int MaxRandomNumberRerolls { get; set; } = 5;
}