using System;
using System.Diagnostics;

namespace RandomPicker.App.Services;

public class BrowserService
{
    private readonly IBrowserLauncher _browserLauncher;

    public BrowserService() : this(new SystemBrowserLauncher())
    {
        
    }
    public BrowserService(IBrowserLauncher browserLauncher)
    {
        _browserLauncher = browserLauncher;
    }
    
    public void OpenUrlInDefaultBrowser(string url)
    {
        try
        {
            _browserLauncher.OpenUrlInBrowser(url);
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Error opening URL in default browser: {ex.Message}");
            throw;
        }
    }
}