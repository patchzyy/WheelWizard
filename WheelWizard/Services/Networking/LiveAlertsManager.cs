using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CT_MKWII_WPF.Views.Components;
using MahApps.Metro.IconPacks;

namespace CT_MKWII_WPF.Services.Networking
{
    public static class LiveAlertsManager
    {
        private static readonly string StatusUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/status.txt";
        private static DynamicIcon _statusIcon;
        private static Action<string> _updateStatusMessage;
        private static readonly DispatcherTimer Timer = new() { Interval = TimeSpan.FromSeconds(60) };
        private static readonly HttpClient HttpClient = new();

        public static void Start(DynamicIcon statusIcon,  Action<string> updateStatusMessage)
        {
            _statusIcon = statusIcon;
            _updateStatusMessage = updateStatusMessage;
            Timer.Tick += async (s, e) => await UpdateStatus(); 
            
            _statusIcon.IconKind = null;
            _statusIcon.ForegroundColor = null;
            _updateStatusMessage("");
            
            Timer.Start();
            Task.Run(async () => await UpdateStatus());
        }

        public static void Stop() => Timer.Stop();

        private static async Task UpdateStatus()
        {
            try
            {
                var content = await HttpClient.GetStringAsync(StatusUrl);
                var parts = content.Split("\n");
                var firstLine = parts[0];
                if (string.IsNullOrWhiteSpace(firstLine))
                    return;   //do nothing
                var alertMessage = firstLine.Split("|");
                if (alertMessage.Length == 2)
                {
                    var messageType = alertMessage[0];
                    var message = alertMessage[1];
                    message = Regex.Replace(message, @"\\u000A", "\n");
                    
                    await UpdateUi(messageType, message);
                }
                else
                {
                    await UpdateUi("warning", "Invalid status format received" + alertMessage);
                }
            }
            catch (Exception ex)
            {
                await UpdateUi("error", $"Failed to update status: {ex.Message}");
            }
        }

        private static async Task UpdateUi(string messageType, string message)
        {
            await _statusIcon.Dispatcher.InvokeAsync(() =>
            {
                SolidColorBrush? brush;
                switch (messageType.ToLower())
                {
                    case "party":
                        _statusIcon.IconPack = "Material";
                        _statusIcon.IconKind = PackIconMaterialKind.PartyPopper;
                        brush = Application.Current.FindResource("PartyColor") as SolidColorBrush;
                        break;
                    case "info":
                        _statusIcon.IconPack = "FontAwesome";
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleInfoSolid;
                        brush = Application.Current.FindResource("InfoColor") as SolidColorBrush;
                        break;
                    case "warning":
                    case "alert":
                        _statusIcon.IconPack = "FontAwesome";
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleExclamationSolid;
                        brush = Application.Current.FindResource("WarningColor") as SolidColorBrush;
                        break;
                    case "error":
                        _statusIcon.IconPack = "FontAwesome";
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleXmarkSolid;
                        brush = Application.Current.FindResource("ErrorColor") as SolidColorBrush;
                        break;
                    case "success":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleCheckSolid;
                        brush = Application.Current.FindResource("SuccessColor") as SolidColorBrush;
                        break;
                    default:
                        _statusIcon.IconPack = "FontAwesome";
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleQuestionSolid;
                        brush = Application.Current.FindResource("InfoColor") as SolidColorBrush;
                        break;
                }
                // We never actually want Brushes.Gray,
                // but just in case the resource is missing for some unknown reason, we still want to display the icon
                _statusIcon.ForegroundColor = brush ?? Brushes.Gray;

                _updateStatusMessage(message);
            });
        }
    }
}
