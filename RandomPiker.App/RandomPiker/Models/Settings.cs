namespace RandomPiker.Models;

public class Settings
{
    public bool OpenFileAfterExit { get; set; }
    public bool RandomWithoutRepetitions { get; set; }
    public string PathToFileWithUrls { get; set; }
    public string PathToFileWithCompleted { get; set; }
    public string ApiKey { get; set; }
    public string ApplicationName { get; set; }
}