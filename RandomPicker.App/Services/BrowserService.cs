using System;
using System.Diagnostics;

namespace RandomPicker.App.Services;

public class BrowserService
{
    public void OpenUrlInDefaultBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });


        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Error opening URL in default browser: {ex.Message}");
            throw;
        }
    }
}