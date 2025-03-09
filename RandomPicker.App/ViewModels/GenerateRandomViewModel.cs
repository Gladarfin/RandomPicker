using System;
using System.ComponentModel;
using System.Windows.Input;
using RandomPicker.App.Models;
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
    public ICommand RerollRandomNumberCommand { get; }
    public GenerateRandomViewModel()
    {
        _random = new Random();
        IsRollButtonEnabled = true;
        IsRerollButtonEnabled = false;
        GenerateRandomNumberCommand = ReactiveCommand.Create(GenerateRandomNumber);
        RerollRandomNumberCommand = ReactiveCommand.Create(RerollRandomNumber);
        MessageBus.Current.Listen<VideoCountMessage>().Subscribe(message => RandomMaxValue = message.Count);
    }

    private void GenerateRandomNumber()
    {
        RandomNumber = _random.Next(1, _randomMaxValue);
        IsRollButtonEnabled = false;
        IsRerollButtonEnabled = true;
        SendMessageWithRandomNumber();
    }

    private void RerollRandomNumber()
    {
        RandomNumber = _random.Next(1, _randomMaxValue);
        SendMessageWithRandomNumber();
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