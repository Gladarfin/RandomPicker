using System.Diagnostics;

namespace RandomPicker.App.Services;

public class SystemBrowserLauncher : IBrowserLauncher
{
    public void OpenUrlInBrowser(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}