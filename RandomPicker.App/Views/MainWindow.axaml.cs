using Avalonia.Controls;
using RandomPicker.App.ViewModels;

namespace RandomPicker.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && !viewModel.IsExiting())
        {
            viewModel.ExitCommand.Execute(null);
        }
        
        e.Cancel = true;
        base.OnClosing(e);
    }
}