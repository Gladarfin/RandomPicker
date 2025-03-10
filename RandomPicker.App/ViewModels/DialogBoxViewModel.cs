using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DialogHostAvalonia;
using ReactiveUI;

namespace RandomPicker.App.ViewModels;

public class DialogBoxViewModel : ViewModelBase
{
    public ICommand OpenDialogCommand { get; }
    
    public DialogBoxViewModel()
    {
        OpenDialogCommand = ReactiveCommand.CreateFromTask(ExecuteOpenDialogBoxCommandAsync, outputScheduler: AvaloniaScheduler.Instance);
    }
    
    public ICommand CloseDialogBoxCommand { get; } = ReactiveCommand.Create(() =>
    {
        DialogHost.Close("MainDialogHost");
        
    }, outputScheduler: AvaloniaScheduler.Instance);
    
    public async Task ExecuteOpenDialogBoxCommandAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DialogHost.Show(this, "MainDialogHost");
        }); 
    }
}