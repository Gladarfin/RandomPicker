namespace RandomPicker.App.Models;

public class DialogBoxWithChoiceClosedWithMessage(bool choice)
{
    public bool DialogBoxChoiceIs { get; set; } = choice;
}