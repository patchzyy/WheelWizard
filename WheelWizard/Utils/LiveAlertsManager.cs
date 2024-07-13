using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using CT_MKWII_WPF.Views.Components;
using MahApps.Metro.IconPacks;

namespace CT_MKWII_WPF.Utils
{
    public class LiveAlertsManager
    {
        private readonly string _statusUrl;
        private readonly DynamicIcon _statusIcon;
        private readonly DispatcherTimer _timer;
        private readonly HttpClient _httpClient;
        private readonly Action<string> _updateStatusMessage;

        public LiveAlertsManager(string statusUrl, DynamicIcon statusIcon, Action<string> updateStatusMessage)
        {
            _statusUrl = statusUrl;
            _statusIcon = statusIcon;
            _updateStatusMessage = updateStatusMessage;
            _httpClient = new HttpClient();
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(60) // update the status every 60 seconds
            };
            _timer.Tick += async (s, e) => await UpdateStatus(); //subscribe to the timer tick event
            //so i didnt even know this was a thing but now every timer tick, it will call the UpdateStatus method

            // Initialize with a default state
            InitializeDefaultState();
        }

        private void InitializeDefaultState()
        {
            _statusIcon.IconKind = null;
            _statusIcon.ForegroundColor = null;
            _updateStatusMessage("");
        }

        public void Start()
        {
            _timer.Start();
            Task.Run(async () => await UpdateStatus());
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private async Task UpdateStatus()
        {
            try
            {
                string content = await _httpClient.GetStringAsync(_statusUrl);
                var parts = content.Split("\n");
                var firstline = parts[0];
                if (string.IsNullOrWhiteSpace(firstline))
                    return;   //do nothing
                var alertMessage = firstline.Split("|");
                if (alertMessage.Length == 2)
                {
                    string messageType = alertMessage[0];
                    string message = alertMessage[1];
                    message = Regex.Replace(message, @"\\u000A", "\n");
                    
                    await UpdateUI(messageType, message);
                }
                else
                {
                    await UpdateUI("warning", "Invalid status format received" + alertMessage);
                }
            }
            catch (Exception ex)
            {
                await UpdateUI("error", $"Failed to update status: {ex.Message}");
            }
        }

        private async Task UpdateUI(string messageType, string message)
        {
            await _statusIcon.Dispatcher.InvokeAsync(() =>
            {
                SolidColorBrush? brush;
                switch (messageType.ToLower())
                {
                    case "info":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleInfoSolid;
                        brush = Application.Current.FindResource("InfoColor") as SolidColorBrush;
                        break;
                    case "warning":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleExclamationSolid;
                       brush = Application.Current.FindResource("WarningColor") as SolidColorBrush;
                        break;
                    case "alert":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleExclamationSolid;
                        brush = Application.Current.FindResource("WarningColor") as SolidColorBrush;
                        break;
                    case "error":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleXmarkSolid;
                        brush = Application.Current.FindResource("ErrorColor") as SolidColorBrush;
                        break;
                    case "success":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleCheckSolid;
                        brush = Application.Current.FindResource("SuccessColor") as SolidColorBrush;
                        break;
                    default:
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