using Avalonia.Interactivity;
using System.Media;
using Button = WheelWizard.Views.Components.Button;

namespace WheelWizard.Views.Popups.Generic;

public partial class MessageBoxWindow : PopupContent
{
    public enum MessageType
    {
        Error,
        Warning,
        Message
    }
    private MessageType messageType = MessageType.Message;
    
    public MessageBoxWindow() : base(true, false, true, "Message")
    {
        InitializeComponent();
        CancelButton.Variant = Button.ButtonsVariantType.Primary;
    }
    
    public MessageBoxWindow SetMessageType(MessageType newType)
    {
        messageType = newType;
        if (messageType == MessageType.Message)
            CancelButton.Variant = Button.ButtonsVariantType.Primary;
        Window.Title = messageType.ToString();
        return this;
    }
    
    public MessageBoxWindow SetMainText(string mainText)
    {
        MainTextBlock.Text = mainText;
        return this;
    }
    
    public MessageBoxWindow SetExtraText(string extraText)
    {
        ExtraTextBlock.Text = extraText;
        return this;
    }

    protected override void BeforeOpen() => PlaySound(messageType);
    
    private static void PlaySound(MessageType messageType)
    {
        switch (messageType)
        {
            //todo: fix sounds for all platforms
            case MessageType.Error:
                // SystemSounds.Hand.Play();
                break;
            case MessageType.Warning:
                // SystemSounds.Exclamation.Play();
                break;
            case MessageType.Message:
                // SystemSounds.Hand.Play();
                break;
        }
    }
    
    private void OkButton_Click(object sender, RoutedEventArgs e) => Close();
}

