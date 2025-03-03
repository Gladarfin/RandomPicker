namespace RandomPiker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public double DropDownHeight { get; set; } = 150;
}