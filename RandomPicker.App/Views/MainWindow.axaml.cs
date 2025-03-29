using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
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
            viewModel.OnWindowClosing();
        }
        
        e.Cancel = true;
        base.OnClosing(e);
    }

}