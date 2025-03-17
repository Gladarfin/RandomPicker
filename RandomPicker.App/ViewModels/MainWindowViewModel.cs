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
    private bool _isExiting;
    private readonly string _pathToSettingsFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\Settings.json"));
    private string _pathToCompletedList;
    private int _currentRandomNumber;
    private bool _settingIsChanged = false;
    private string _clipboardText;
    private static CompletedVideosService _completedVideosService;

   //public
    public static Settings AppSettings { get; private set; }
    public bool IsExiting() => _isExiting;

    //Commands
    public ICommand ExitCommand { get; }
    public ICommand TextBlockClickCommand { get; }
    public ICommand ResetCompletedVideosListAsyncCommand { get; }
    public ICommand UpdateCompletedVideosCommand { get; }
    
    //ViewModels
    public GenerateRandomViewModel GenerateRandomVM { get; }
    public YoutubeServiceViewModel YoutubeServiceVM { get; }
    public DialogBoxViewModel DialogBoxVM { get; }
    
    public MainWindowViewModel()
    {   //Don't like that I'm using SettingsService in each ViewModel where I need it
        //TODO: try to find solution: load in App maybe?
        Task.Run(async () =>
        {
            var settingsService = new SettingsService(_pathToSettingsFile);
            AppSettings = await settingsService.LoadSettingsAsync();
        }).Wait();

        _pathToCompletedList = AppSettings.PathToFileWithCompleted;
        GenerateRandomVM = new GenerateRandomViewModel();
        DialogBoxVM = new DialogBoxViewModel();
        YoutubeServiceVM = new YoutubeServiceViewModel();
        SubscribeToMessages();
        ExitCommand = ReactiveCommand.Create(ExecuteExitApplicationCommand);
        TextBlockClickCommand = ReactiveCommand.Create(ExecuteTextBlockClickCommand);
        UpdateCompletedVideosCommand = ReactiveCommand.Create(() => _completedVideosService.UpdateCompletedVideosList());
        ResetCompletedVideosListAsyncCommand = ReactiveCommand.CreateFromTask(async() => await _completedVideosService.ResetListAsync());
        YoutubeServiceVM.CheckAndDeserializeFile(AppSettings.PathToFileWithUrls);
    }

    private void SubscribeToMessages()
    {
        MessageBus.Current.Listen<VideoUrlMessage>().Subscribe(message =>
        {
            _clipboardText = message.Url;
        });
        MessageBus.Current.Listen<RandomNumberMessage>().Subscribe(message =>
        {
            _currentRandomNumber = message.RandomNumber;
            UpdateCompletedVideosService();
        });
        MessageBus.Current.Listen<FileNotFoundMessage>().Subscribe(message => 
                DialogBoxVM.OpenDialogCommand.Execute($"File doesn't exist:\n {message.PathToFile}")
                );
    }
    
    private void ExecuteExitApplicationCommand()
    {
        if (_isExiting) return;
        _isExiting = true;
        
         if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
         {
            if (!GenerateRandomVM.IsRollButtonEnabled)
                UpdateCompletedVideosCommand.Execute(null);
            desktop.Shutdown();
         }
        _isExiting = false;
    }
    
    private void ExecuteTextBlockClickCommand()
    {
        //copy to clipboard
        var clipboard = ClipboardService.Get();
        Task.Run(async() => await clipboard.SetTextAsync(_clipboardText));
        DialogBoxVM.OpenDialogWithAutoCloseCommand.Execute(null);
    }
    
    private void UpdateCompletedVideosService()
    {
        _completedVideosService = new CompletedVideosService(_pathToCompletedList, _currentRandomNumber);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}