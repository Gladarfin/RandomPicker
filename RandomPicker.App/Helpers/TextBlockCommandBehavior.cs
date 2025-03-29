using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace RandomPicker.App.Services;

public class TextBlockCommandBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<TextBlockCommandBehavior, Interactive, ICommand>(
            "Command");

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
                interactive.AddHandler(InputElement.PointerPressedEvent, (_, _) =>
                {
                    var command = GetCommand(interactive);
                        command.Execute(null);
                });
            }
        });
    }
}