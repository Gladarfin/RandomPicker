namespace RandomPicker.App.Models;

public class DialogBoxClosedWithYesMessage(bool choice)
{
    public bool DialogBoxChoiceIs { get; set; } = choice;
}