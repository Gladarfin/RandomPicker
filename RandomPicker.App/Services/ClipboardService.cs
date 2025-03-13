using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.VisualTree;

namespace RandomPicker.App.Services;

public class ClipboardService
{
        public static IClipboard Get() {

            //Desktop
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window }) {
                return window.Clipboard!;
            }
            return null!;
        }
}