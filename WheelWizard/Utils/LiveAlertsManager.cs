using System;
using System.Net.Http;
using System.Threading.Tasks;
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
                {
                    //do nothing
                    return;
                }
                var alertMessage = firstline.Split("|");
                if (alertMessage.Length == 2)
                {
                    string messageType = alertMessage[0];
                    string message = alertMessage[1];

                    await UpdateUI(messageType, message);
                }
                else
                {
                    await UpdateUI("warning", "Invalid status format received" + alertMessage);
                }
            }
            catch (Exception ex)
            {
                await UpdateUI("alert", $"Failed to update status: {ex.Message}");
            }
        }

        private async Task UpdateUI(string messageType, string message)
        {
            await _statusIcon.Dispatcher.InvokeAsync(() =>
            {
                switch (messageType.ToLower())
                {
                    case "info":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleInfoSolid;
                        _statusIcon.ForegroundColor = Brushes.Gray;
                        break;
                    case "warning":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.TriangleExclamationSolid;
                        _statusIcon.ForegroundColor = Brushes.Orange;
                        break;
                    case "alert":
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleExclamationSolid;
                        _statusIcon.ForegroundColor = Brushes.Red;
                        break;
                    default:
                        _statusIcon.IconKind = PackIconFontAwesomeKind.CircleQuestionSolid;
                        _statusIcon.ForegroundColor = Brushes.Gray;
                        break;
                }

                _updateStatusMessage(message);
            });
        }
    }
}