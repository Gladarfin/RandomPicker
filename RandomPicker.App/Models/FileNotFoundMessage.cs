namespace RandomPicker.App.Models;

public class FileNotFoundMessage(string pathToFile)
{
    public string PathToFile { get; } = pathToFile;
}