using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CT_MKWII_WPF.Services.WheelWizard;

public static class LiveAlertsManager
{
    private const string StatusUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/status.txt";
    
    private static Action<string, string>? _updateStatusMessage;
    private static readonly DispatcherTimer Timer = new() { Interval = TimeSpan.FromSeconds(60) };

    public static void Start( Action<string, string> updateStatusMessage)
    {
        _updateStatusMessage = updateStatusMessage;
        Timer.Tick += async (_, _) => await UpdateStatusAsync();
        
        _updateStatusMessage("", "");

        Timer.Start();
        Task.Run(UpdateStatusAsync);
    }

    public static void Stop() => Timer.Stop();

    private static (string, string) ParseStatus(string status)
    {
        var parts = status.Split("\n");
        var firstLine = parts.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstLine))
            return ("","");
        
        var firstLineParts = firstLine.Split("|");
        if (firstLineParts.Length != 2)
            return  ("","");
          
        var messageType = firstLineParts[0];
        var message = firstLineParts[1];
        message = Regex.Replace(message, @"\\u000A", "\n");
        return (messageType, message);
    }
    
    private static async Task UpdateStatusAsync()
    {
        var response = await HttpClientHelper.GetAsync<string>(StatusUrl);
        if (!response.Succeeded || response.Content is null)
        {
            // We DONT want to show anything if the request failed.
            // There is no use-case for this and it will only confuse the user.
            _updateStatusMessage?.Invoke("", "");
            return;
        }
        
        var (messageType, message) = ParseStatus(response.Content);
        Application.Current.Dispatcher.Invoke(() => _updateStatusMessage?.Invoke(messageType, message));
    }
}