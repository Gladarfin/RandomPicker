using System.Windows.Input;
using Avalonia.ReactiveUI;
using DialogHostAvalonia;
using ReactiveUI;

namespace RandomPicker.App.ViewModels;

public class DialogBoxViewModel : ViewModelBase
{
    public ICommand CloseDialogBoxCommand { get; } = ReactiveCommand.Create(() =>
    {
        DialogHost.Close("MainDialogHost");
        
    }, outputScheduler: AvaloniaScheduler.Instance);
}