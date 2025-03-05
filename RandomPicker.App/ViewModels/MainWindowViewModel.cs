using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;


namespace RandomPicker.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private bool _openFileAfterExit;
    private bool _withoutRepetition;
    private string _pathToFile = "Path not set";
    private bool _isExiting = false;
    
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
    
    public double DropDownHeight { get; } = 150;
    
    //Commands
    public ICommand ExitCommand { get; }
    
    public MainWindowViewModel()
    {
        ExitCommand = ReactiveCommand.Create(ExitApplication);
    }

    private void ExitApplication()
    {
        if (_isExiting) return;
        _isExiting = true;
        
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("Run!!!");
            desktop.Shutdown();
        }

        _isExiting = false;
    }
    
    public bool IsExiting() => _isExiting;
    
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