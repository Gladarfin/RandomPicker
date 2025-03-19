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
    private readonly string _pathToCompletedList;
    private int _currentRandomNumber;
    private string _clipboardText;
    private static CompletedVideosService _completedVideosService;
    private static Settings _appSettings;
    private static BrowserService _browserService;
    
   //public
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
    
    public MainWindowViewModel(SettingsService settingsService,
        BrowserService browserService,
        YoutubeServiceViewModel youtubeServiceViewModel,
        DialogBoxViewModel dialogBoxViewModel,
        GenerateRandomViewModel generateRandomViewModel)
    {
        Task.Run(async () => {
            _appSettings = await settingsService.LoadSettingsAsync();
        }).Wait();
        _pathToCompletedList = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), _appSettings.PathToFileWithCompleted);
        var pathToFileWithUrl = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), _appSettings.PathToFileWithUrls);
        GenerateRandomVM = generateRandomViewModel;
        DialogBoxVM = dialogBoxViewModel;
        YoutubeServiceVM = youtubeServiceViewModel;
        _browserService = browserService;
        SubscribeToMessages();
        ExitCommand = ReactiveCommand.Create(ExecuteExitApplicationCommand);
        TextBlockClickCommand = ReactiveCommand.Create(ExecuteTextBlockClickCommand);
        UpdateCompletedVideosCommand = ReactiveCommand.Create(() => _completedVideosService.UpdateCompletedVideosList());
        ResetCompletedVideosListAsyncCommand = ReactiveCommand.CreateFromTask(async() => await _completedVideosService.ResetListAsync());
        YoutubeServiceVM.CheckAndDeserializeUrlsFile(pathToFileWithUrl);
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
             if (GenerateRandomVM.IsRollButtonEnabled)
                 desktop.Shutdown();
             try
             {
                 if (!GenerateRandomVM.IsRollButtonEnabled)
                     UpdateCompletedVideosCommand.Execute(null);
                 
                 if (_clipboardText != string.Empty && _appSettings.OpenFileAfterExit)
                    _browserService.OpenUrlInDefaultBrowser(_clipboardText);
                 
                 desktop.Shutdown();

             }
             catch (Exception ex)
             {
                 DialogBoxVM.OpenDialogCommand.Execute($"Error opening URL in default browser: {ex.Message}");
                 _isExiting = false;
                 return;
             }
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
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}