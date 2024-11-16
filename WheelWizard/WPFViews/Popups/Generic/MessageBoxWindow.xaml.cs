using System.Media;
using System.Windows;
using WheelWizard.Views.Components;

namespace WheelWizard.WPFViews.Popups.Generic;

public partial class MessageBoxWindow : PopupContent
{
    public enum MessageType
    {
        Error,
        Warning,
        Message
    }

    private MessageBoxWindow(string message, MessageType messageType) : base(true, false, true, GetTitle(messageType))
    {
        InitializeComponent();
        ErrorTextBlock.Text = message;
        PlaySound(messageType);
        if (messageType == MessageType.Message)
        {
            CancelButton.Variant = Button.ButtonsVariantType.Primary;
        }
            
    }

    public static void ShowDialog(string message, MessageType messageType = MessageType.Message)
    {
        var messageBoxWindow = new MessageBoxWindow(message, messageType);
        messageBoxWindow.ShowDialog();
    }
    
    public static void Show(string message, MessageType messageType = MessageType.Message)
    {
        var messageBoxWindow = new MessageBoxWindow(message, messageType);
        messageBoxWindow.Show();
    }
    

    private static string GetTitle(MessageType messageType)
    {
        return messageType switch
        {
            MessageType.Error => "Error",
            MessageType.Warning => "Warning",
            MessageType.Message => "Message",
            _ => "Message"
        };
    }

    private static void PlaySound(MessageType messageType)
    {
        switch (messageType)
        {
            case MessageType.Error:
                SystemSounds.Hand.Play();
                break;
            case MessageType.Warning:
                SystemSounds.Exclamation.Play();
                break;
            case MessageType.Message:
                SystemSounds.Hand.Play();
                break;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

