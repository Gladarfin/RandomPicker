using System.ComponentModel;

namespace RandomPicker.App.ViewModels;

public class SettingsViewModel
{
    private bool _openFileAfterExit;

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
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}