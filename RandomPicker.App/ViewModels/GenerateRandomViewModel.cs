using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using RandomPicker.App.Models;
using RandomPicker.App.Services;
using ReactiveUI;

namespace RandomPicker.App.ViewModels;

public class GenerateRandomViewModel : INotifyPropertyChanged
{
    //private
    private readonly Random _random;
    private int _randomNumber;
    private int _randomMaxValue = 100;
    private bool _isRollButtonEnabled;
    private bool _isRerollButtonEnabled;
    private static Settings _appSettings;
    private readonly DialogBoxViewModel _dialogBoxViewModel;
    private readonly string _pathToFileWithCompleted;
    private int _currentRollsCount;
    private bool _isReseted = false;
    private CompletedVideosService _completedVideosService;
    
    //public
    public int RandomNumber
    {
        get => _randomNumber;
        private set
        {
            _randomNumber = value;
            OnPropertyChanged(nameof(RandomNumber));
        }
    }
    public int RandomMaxValue
    {
        get => _randomMaxValue;
        private set
        {
            _randomMaxValue = value;
            OnPropertyChanged(nameof(RandomMaxValue));
        }
    }
    public bool IsRollButtonEnabled
    {
        get => _isRollButtonEnabled;
        private set
        {
            _isRollButtonEnabled = value;
            OnPropertyChanged(nameof(IsRollButtonEnabled));
        }
    }
    public bool IsRerollButtonEnabled
    {
        get => _isRerollButtonEnabled;
        private set
        {
            _isRerollButtonEnabled = value;
            OnPropertyChanged(nameof(IsRerollButtonEnabled));
        }
    }
    
    //Commands
    public ICommand GenerateRandomNumberCommand { get; }
    public ICommand RerollRandomNumberCommandAsync { get; }
    public GenerateRandomViewModel(SettingsService settingsService,
        DialogBoxViewModel dialogBoxViewModel)
    {
        _random = new Random();
        IsRollButtonEnabled = true;
        IsRerollButtonEnabled = false;
        Task.Run(async () => {
            _appSettings = await settingsService.LoadSettingsAsync();
        }).Wait();
        _dialogBoxViewModel = dialogBoxViewModel;
        _pathToFileWithCompleted  = Path.Combine(
            Path.GetDirectoryName(Environment.ProcessPath), 
            _appSettings.PathToFileWithCompleted);
        
        GenerateRandomNumberCommand = ReactiveCommand.Create(GenerateRandomNumber);
        RerollRandomNumberCommandAsync = ReactiveCommand.CreateFromTask(RerollRandomNumberAsync);
        SubscribeToMessages();
    }
    
    private void SubscribeToMessages()
    {
        MessageBus.Current.Listen<VideoCountMessage>().Subscribe(message => RandomMaxValue = message.Count);
        MessageBus.Current.Listen<ThumbnailLoadFailedMessage>().Subscribe(_ => Task.Run(async() => await RerollRandomNumberAsync()));
    }
    private void GenerateRandomNumber()
    {
        RollNewRandomNumber();
        IsRollButtonEnabled = false;
        IsRerollButtonEnabled = true;
        _currentRollsCount++;
        SendMessageWithRandomNumber();
    }

    private async Task RerollRandomNumberAsync()
    {
        if (CheckIfCurrentRollsExceededMaxRerolls())
        {
            if (CheckIfReseted())
                return;

            if (!await CheckForUserAnswerInDialogBoxAsync())
                return;
            ResetCompletedList();
        }
        
        RollNewRandomNumber();
        _currentRollsCount++;
        SendMessageWithRandomNumber();
    }

    private bool CheckIfCurrentRollsExceededMaxRerolls()
    {
        return _currentRollsCount > _appSettings.MaxRandomNumberRerolls;
    }

    private bool CheckIfReseted()
    {
        if (!_isReseted) return false;
        
        _dialogBoxViewModel.OpenDialogCommandAsync.Execute("Random number can't be generated.");
        return true;
    }

    private async Task<bool> CheckForUserAnswerInDialogBoxAsync()
    {
        _dialogBoxViewModel.OpenDialogWithChoiceCommandAsync.Execute(
            """
            Random number can't be generated; maybe you have completed all the videos from the given playlists.
            Would you like to try resetting the CompletedList and attempt a reroll?
            """);
        var userChoice = await MessageBus.Current.Listen<DialogBoxWithChoiceClosedWithMessage>().FirstAsync();
        return !userChoice.DialogBoxChoiceIs;
    }

    private void ResetCompletedList()
    {
        _currentRollsCount = 0;
        _completedVideosService = new CompletedVideosService(_appSettings.PathToFileWithCompleted);
        Task.Run(async() => await _completedVideosService.ResetListAsync());
    }

    private void RollNewRandomNumber()
    {
        if (_appSettings.RandomWithoutRepetitions && !IsFileExists(_pathToFileWithCompleted))
        {
            _dialogBoxViewModel.OpenDialogCommandAsync.Execute($"File doesn't exist:\n {_pathToFileWithCompleted}");
            return;
        }
        
        var json = JsonConvert.DeserializeObject<CompletedVideos>(File.ReadAllText(_pathToFileWithCompleted));
        
        while (true)
        {
            if (!_appSettings.RandomWithoutRepetitions)
            {
                RandomNumber = _random.Next(1, _randomMaxValue);
                break;
            }
            RandomNumber = _random.Next(1, _randomMaxValue);
            if (!json.CompletedList.Contains(RandomNumber))
                break;
        }
    }
        
    private static bool IsFileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    private void SendMessageWithRandomNumber()
    {
        MessageBus.Current.SendMessage(new RandomNumberMessage(RandomNumber));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}