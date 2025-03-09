using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DialogHostAvalonia;
using ReactiveUI;


namespace RandomPicker.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    //private
    private bool _openFileAfterExit;
    private bool _withoutRepetition;
    private string _pathToFile = "Path not set";
    private bool _isExiting;

   //public
    public bool OpenFileAfterExit
    {
        get => _openFileAfterExit;
        set
        {
            if (_openFileAfterExit == value) return;
            
            _openFileAfterExit = value;
            OnPropertyChanged(nameof(OpenFileAfterExit));
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
        }
    }
    public double DropDownHeight => 150;

    //Commands
    public ICommand ExitCommand { get; }
    public ICommand GenerateRandomNumberCommand { get; }
    public ICommand OpenDialogCommand { get; }
    
    //ViewModels
    public GenerateRandomViewModel GenerateRandomVM { get; }
    public YoutubeServiceViewModel YoutubeServiceVM { get; }
    public DialogBoxViewModel DialogBoxVM { get; }

    public MainWindowViewModel()
    {
        GenerateRandomVM = new GenerateRandomViewModel();
        YoutubeServiceVM = new YoutubeServiceViewModel();
        DialogBoxVM = new DialogBoxViewModel();
        ExitCommand = ReactiveCommand.Create(ExecuteExitApplicationCommand);
        OpenDialogCommand = ReactiveCommand.CreateFromTask(ExecuteOpenDialogBoxCommandAsync, outputScheduler: AvaloniaScheduler.Instance);
        GenerateRandomNumberCommand = ReactiveCommand.Create(ExecuteGenerateRandomNumberCommand);
    }

    private void ExecuteExitApplicationCommand()
    {
        if (_isExiting) return;
        _isExiting = true;
        
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            //Some logic before close the app, like save settings if changed etc
            desktop.Shutdown();
        }

        _isExiting = false;
    }

    private void ExecuteGenerateRandomNumberCommand()
    {
        GenerateRandomVM.GenerateRandomNumberCommand.Execute(null);
    }

    public async Task ExecuteOpenDialogBoxCommandAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DialogHost.Show(DialogBoxVM, "MainDialogHost");
        }); 
    }
    
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
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}