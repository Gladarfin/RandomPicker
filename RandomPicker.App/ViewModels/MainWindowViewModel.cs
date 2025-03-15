using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using RandomPicker.App.Models;
using RandomPicker.App.Services;
using ReactiveUI;
using Tmds.DBus.Protocol;


namespace RandomPicker.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    //private
    private bool _openFileAfterExit;
    private bool _withoutRepetition;
    private bool _isExiting;
    private string _pathToFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\Settings.json"));
    private string _pathToCompletedList = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\CompletedVideos.json"));
    private bool _settingIsChanged = false;
    private string _clipboardText;
    private bool _tooltipIsVisible = false;
    private int _currentRandomNumber;

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
    public bool TooltipIsVisible
    {
        get => _tooltipIsVisible;
        set
        {
            if (_tooltipIsVisible == value) return;
            _tooltipIsVisible = value;
            OnPropertyChanged(nameof(TooltipIsVisible));
        }
    }
    public double DropDownHeight => 150;
    public static Settings AppSettings { get; private set; }
    public bool IsExiting() => _isExiting;

    //Commands
    public ICommand ExitCommand { get; }
    public ICommand GenerateRandomNumberCommand { get; }
    public ICommand TextBlockClickCommand { get; }
    public ICommand ResetListCommand { get; }
    
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
        TextBlockClickCommand = ReactiveCommand.Create(ExecuteTextBlockClickCommand);
        ResetListCommand = ReactiveCommand.CreateFromTask(ExecuteResetListCommandAsync);

        MessageBus.Current.Listen<VideoUrlMessage>().Subscribe(message =>
        {
            _clipboardText = message.Url;
        });
        MessageBus.Current.Listen<RandomNumberMessage>().Subscribe(message => _currentRandomNumber = message.RandomNumber);
    }
    private void ExecuteExitApplicationCommand()
    {
        if (_isExiting) return;
        _isExiting = true;
        
         if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
         {
            if (_settingIsChanged)
            {
                ChangeSettings();
                Task.Run(async () => {
                    var settingsService = new SettingsService(PathToFile);
                    await settingsService.SaveSettingsAsync(AppSettings);
                }).Wait();
            }
            UpdateCompletedVideosList();
            desktop.Shutdown();
         }
        _isExiting = false;
    }
    private void ExecuteTextBlockClickCommand()
    {
        //copy to clipboard
        var clipboard = ClipboardService.Get();
        Task.Run(async() => await clipboard.SetTextAsync(_clipboardText));
        TooltipIsVisible = true;
        DialogBoxVM.OpenDialogWithAutoCloseCommand.Execute(null);
    }
    private void ExecuteGenerateRandomNumberCommand()
    {
        GenerateRandomVM.GenerateRandomNumberCommand.Execute(null);
    }
    private async Task ExecuteResetListCommandAsync()
    {
        var newList = JsonConvert.SerializeObject(new CompletedVideos([]), Formatting.Indented); 
        await File.WriteAllTextAsync(_pathToCompletedList, newList);
        Debug.WriteLine("List of completed videos was reset");
    }
    private void ChangeSettings()
    {
        AppSettings.OpenFileAfterExit = _openFileAfterExit;
        AppSettings.RandomWithoutRepetitions = _withoutRepetition;
    }

    private void UpdateCompletedVideosList()
    {
        if (!File.Exists(_pathToCompletedList))
        {
            Task.Run(async () => await ExecuteResetListCommandAsync()).Wait();
        }

        var json = JsonConvert.DeserializeObject<CompletedVideos>(File.ReadAllText(_pathToCompletedList));
        json.CompletedList.Add(_currentRandomNumber);
        var updatedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
        Task.Run(async() => await File.WriteAllTextAsync(_pathToCompletedList, updatedJson)).Wait();
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}