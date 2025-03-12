using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using RandomPicker.App.Models;
using RandomPicker.App.Services;
using ReactiveUI;


namespace RandomPicker.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    //private
    private bool _openFileAfterExit;
    private bool _withoutRepetition;
    private bool _isExiting;
    private string _pathToFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\Settings.json"));
    private bool _settingIsChanged = false;

   //public
    public bool OpenFileAfterExit
    {
        get => _openFileAfterExit;
        set
        {
            if (_openFileAfterExit == value) return;
            
            _openFileAfterExit = value;
            OnPropertyChanged(nameof(OpenFileAfterExit));
            _settingIsChanged = true;
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
            _settingIsChanged = true;
        }
    }
    
    public double DropDownHeight => 150;
    public static Settings AppSettings { get; private set; }
        
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

    //Commands
    public ICommand ExitCommand { get; }
    public ICommand GenerateRandomNumberCommand { get; }
    
    //ViewModels
    public GenerateRandomViewModel GenerateRandomVM { get; }
    public YoutubeServiceViewModel YoutubeServiceVM { get; }
    public DialogBoxViewModel DialogBoxVM { get; }
    
    public MainWindowViewModel()
    {   //Don't like that I'm using SettingsService in each ViewModel where I need it
        //TODO: try to find solution: load in App maybe?
        Task.Run(async () =>
        {
            var settingsService = new SettingsService(PathToFile);
            AppSettings = await settingsService.LoadSettingsAsync();
        }).Wait();
        
        _openFileAfterExit = AppSettings.OpenFileAfterExit;
        _withoutRepetition = AppSettings.RandomWithoutRepetitions;
        GenerateRandomVM = new GenerateRandomViewModel();
        YoutubeServiceVM = new YoutubeServiceViewModel();
        DialogBoxVM = new DialogBoxViewModel();
        ExitCommand = ReactiveCommand.Create(ExecuteExitApplicationCommand);
        GenerateRandomNumberCommand = ReactiveCommand.Create(ExecuteGenerateRandomNumberCommand);
    }

    private void ExecuteExitApplicationCommand()
    {
        if (_isExiting) return;
        _isExiting = true;
        
         if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
         {
            //Some logic before close the app, like save settings if changed etc
            if (_settingIsChanged)
            {
                ChangeSettings();
                Task.Run(async () => {
                    var settingsService = new SettingsService(PathToFile);
                    await settingsService.SaveSettingsAsync(AppSettings);
                }).Wait();
            }
            desktop.Shutdown();
        }
        _isExiting = false;
    }

    private void ChangeSettings()
    {
        AppSettings.OpenFileAfterExit = _openFileAfterExit;
        AppSettings.RandomWithoutRepetitions = _withoutRepetition;
    }

    private void ExecuteGenerateRandomNumberCommand()
    {
        GenerateRandomVM.GenerateRandomNumberCommand.Execute(null);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}