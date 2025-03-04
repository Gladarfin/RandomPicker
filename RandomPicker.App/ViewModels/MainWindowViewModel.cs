using System.ComponentModel;

namespace RandomPicker.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public double DropDownHeight { get; set; } = 150;
    private bool _openFileAfterExit;
    private bool _withoutRepetition;
    private string _pathToFile = "Path not set";
    
    public bool OpenFileAfterExit
    {
        get => _openFileAfterExit;
        set
        {
            if (_openFileAfterExit == value) return;
            
            _openFileAfterExit = value;
            OnPropertyChanged(nameof(OpenFileAfterExit));
        }
    }
    public bool WithoutRepetition
    {
        get => _withoutRepetition;
        set
        {
            if (_withoutRepetition == value) return;
            _withoutRepetition = value;
            OnPropertyChanged(nameof(WithoutRepetition));
        }
    }
    public string PathToFile
    {
        get => _pathToFile;
        set
        {
            if (_pathToFile == value) return;
            _pathToFile = value;
            OnPropertyChanged(nameof(PathToFile));
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}