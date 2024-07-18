using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CT_MKWII_WPF.Views.Components;
using MahApps.Metro.IconPacks;

namespace CT_MKWII_WPF.Services
{
    public static class LiveAlertsManager
    {
        private const string StatusUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/status.txt";

        private static DynamicIcon? _statusIcon;
        private static Action<string>? _updateStatusMessage;
        private static readonly DispatcherTimer Timer = new() { Interval = TimeSpan.FromSeconds(60) };
        private static readonly HttpClient HttpClient = new();

        public static void Start(DynamicIcon statusIcon, Action<string> updateStatusMessage)
        {
            _statusIcon = statusIcon;
            _updateStatusMessage = updateStatusMessage;
            Timer.Tick += async (_, _) => await UpdateStatusAsync();

            _statusIcon.IconKind = null!;
            _statusIcon.ForegroundColor = null!;
            _updateStatusMessage("");

            Timer.Start();
            Task.Run(async () => await UpdateStatusAsync());
        }

        public static void Stop() => Timer.Stop();

        private static async Task UpdateStatusAsync()
        {
            var response = await HttpClientHelper.GetAsync<string>(StatusUrl);
            if (!response.Succeeded || response.Content is null) return; 
            // We DONT want to show anything if the request failed.
            // There is no use-case for this and it will only confuse the user.
            
            var parts = response.Content.Split("\n");
            var firstLine = parts.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(firstLine)) return;
            
            var firstLineParts = firstLine.Split("|");
            if (firstLineParts.Length != 2) return;
            
            var messageType = firstLineParts[0];
            var message = firstLineParts[1];
            message = Regex.Replace(message, @"\\u000A", "\n");

            await UpdateUiAsync(messageType, message);
        }

        // TODO: This method should move to its designated ViewModal, and then be broken up in to the
        //       views code since it is toutching the xaml
        //       also not usre if it has to be async then anymore ¯\(°_o)/¯
        private static async Task UpdateUiAsync(string messageType, string message)
        {
            if (_statusIcon == null) return;
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
                if (_updateStatusMessage == null) return;
                _updateStatusMessage(message);
            });
        }
    }
}