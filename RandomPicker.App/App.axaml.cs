using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RandomPicker.App.Services;
using RandomPicker.App.ViewModels;
using RandomPicker.App.Views;

namespace RandomPicker.App;

public partial class App : Application
{
    private readonly string _pathToSettingsFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Config\Settings.json"));

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
        }
        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SettingsService>(_ => new SettingsService(_pathToSettingsFile));
        services.AddSingleton<BrowserService>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<YoutubeApiService>();
        services.AddTransient<YoutubeServiceViewModel>();
        services.AddTransient<GenerateRandomViewModel>();
        services.AddTransient<DialogBoxViewModel>();
    }
}