using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DialogHostAvalonia;
using ReactiveUI;

namespace RandomPicker.App.ViewModels;

public class DialogBoxViewModel : ViewModelBase
{
    //private
    private const int AutoCloseDelay = 1500;
    private bool _buttonVisibility = false;
    private string _popupText;
    private string _popupIcon;
    
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
    
    //commands
    public ICommand OpenDialogCommand { get; }
    public ICommand OpenDialogWithAutoCloseCommand { get; }
    
    public DialogBoxViewModel()
    {
        OpenDialogCommand = ReactiveCommand.CreateFromTask<string>(ExecuteOpenDialogBoxCommandAsync, outputScheduler: AvaloniaScheduler.Instance);
        OpenDialogWithAutoCloseCommand = ReactiveCommand.CreateFromTask(ExecuteOpenDialogWithAutoCloseCommandAsync, outputScheduler: AvaloniaScheduler.Instance);
    }
    
    public ICommand CloseDialogBoxCommand { get; } = ReactiveCommand.Create(() =>
    {
        DialogHost.Close("MainDialogHost");
        
    }, outputScheduler: AvaloniaScheduler.Instance);
    
    public async Task ExecuteOpenDialogBoxCommandAsync(string odText)
    {
        ButtonVisibility = true;
        PopupText = odText;
        PopupIcon = string.Empty;
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DialogHost.Show(this, "MainDialogHost");
        });
    }

    public async Task ExecuteOpenDialogWithAutoCloseCommandAsync()
    {
        ButtonVisibility = false;
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
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}