using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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
        
    public MessageBoxWindow(string message, MessageType messageType) :
        base(true, false, true, messageType.ToString())
    {
        InitializeComponent();
        ErrorTextBlock.Text = message;
        PlaySound(messageType);
        if (messageType == MessageType.Message)
            CancelButton.Variant = Button.ButtonsVariantType.Primary;
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
    
    private void OkButton_Click(object sender, RoutedEventArgs e) => Close();
}

