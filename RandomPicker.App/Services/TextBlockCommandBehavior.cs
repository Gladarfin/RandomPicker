using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace RandomPicker.App.Services;

public class TextBlockCommandBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<TextBlockCommandBehavior, Interactive, ICommand>(
            "Command", default(ICommand), false, BindingMode.OneWay);

    public static void SetCommand(Interactive element, ICommand value)
    {
        element.SetValue(CommandProperty, value);
    }

    public static ICommand GetCommand(Interactive element)
    {
        return element.GetValue(CommandProperty);
    }

    static TextBlockCommandBehavior()
    {
        CommandProperty.Changed.Subscribe(args =>
        {
            if (args.Sender is Interactive interactive)
            {
                interactive.AddHandler(InputElement.PointerPressedEvent, (sender, e) =>
                {
                    var command = GetCommand(interactive);
                    if (command?.CanExecute(null) == true)
                    {
                        command.Execute(null);
                    }
                });
            }
        });
    }
}