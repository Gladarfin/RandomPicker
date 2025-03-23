using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DialogHostAvalonia;
using RandomPicker.App.Models;
using ReactiveUI;

namespace RandomPicker.App.ViewModels;

public class DialogBoxViewModel : ViewModelBase
{
    //private
    private const int AutoCloseDelay = 1500;
    private bool _buttonVisibility;
    private string _popupText;
    private string _popupIcon;
    private bool _stackPanelWithChoice;
    
    //public
    public bool ButtonVisibility
    {
        get => _buttonVisibility;
        set
        {
            _buttonVisibility = value;
            OnPropertyChanged(nameof(ButtonVisibility));
        }
    }
    public string PopupText
    {
        get => _popupText;
        set
        {
            _popupText = value;
            OnPropertyChanged(nameof(PopupText));
        }
    }
    public string PopupIcon
    {
        get => _popupIcon;
        set
        {
            _popupIcon = value;
            OnPropertyChanged(nameof(PopupIcon));
        }
    }

    public bool StackPanelWithChoice
    {
        get => _stackPanelWithChoice;
        set
        {
            _stackPanelWithChoice = value;
            OnPropertyChanged(nameof(StackPanelWithChoice));
        }
    }
    
    //commands
    public ICommand OpenDialogCommandAsync { get; }
    public ICommand OpenDialogWithAutoCloseCommandAsync { get; }
    public ICommand OpenDialogWithChoiceCommandAsync { get; }
    public ICommand CloseDialogBoxCommand { get; } = ReactiveCommand.Create(() =>
    {
        DialogHost.Close("MainDialogHost");
    }, outputScheduler: AvaloniaScheduler.Instance);

    public ICommand CloseDialogBoxWithChoiceCommand { get; } = ReactiveCommand.Create<bool>((choice) =>
    {
        DialogHost.Close("MainDialogBox");
        MessageBus.Current.SendMessage(new DialogBoxClosedWithYesMessage(choice));
    }, outputScheduler: AvaloniaScheduler.Instance);
    
    public DialogBoxViewModel()
    {
        OpenDialogCommandAsync = ReactiveCommand.CreateFromTask<string>(ExecuteOpenDialogBoxCommandAsync, outputScheduler: AvaloniaScheduler.Instance);
        OpenDialogWithAutoCloseCommandAsync = ReactiveCommand.CreateFromTask(ExecuteOpenDialogWithAutoCloseCommandAsync, outputScheduler: AvaloniaScheduler.Instance);
        OpenDialogWithChoiceCommandAsync = ReactiveCommand.CreateFromTask<string>(ExecuteOpenDialogWithChoiceCommandAsync, outputScheduler:AvaloniaScheduler.Instance);
    }
    
    public async Task ExecuteOpenDialogBoxCommandAsync(string odText)
    {
        ButtonVisibility = true;
        PopupText = odText;
        PopupIcon = string.Empty;
        StackPanelWithChoice = false;
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DialogHost.Show(this, "MainDialogHost");
        });
    }
    
    public async Task ExecuteOpenDialogWithAutoCloseCommandAsync()
    {
        ButtonVisibility = false;
        StackPanelWithChoice = false;
        PopupIcon = "\uf058";
        PopupText = "Link was copied!";
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DialogHost.Show(this, "MainDialogHost");
        });
        await Task.Delay(AutoCloseDelay);
        CloseDialogBoxCommand.Execute(null);
        PopupText = "";
    }
    
    public async Task ExecuteOpenDialogWithChoiceCommandAsync(string odText)
    {
        ButtonVisibility = false;
        PopupText = odText;
        PopupIcon = string.Empty;
        StackPanelWithChoice = true;
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DialogHost.Show(this, "MainDialogHost");
        });
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}