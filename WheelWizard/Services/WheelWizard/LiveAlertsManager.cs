using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Utilities.RepeatedTasks;

namespace CT_MKWII_WPF.Services.WheelWizard;

public class LiveAlertsManager : RepeatedTaskManager
{
    private const string StatusUrl = "https://raw.githubusercontent.com/patchzyy/WheelWizard/main/status.txt";
    public string StatusMessage { get; private set; } = "";
    public string StatusMessageType { get; private set; } = "";

    private static LiveAlertsManager? _instance;
    public static LiveAlertsManager Instance => _instance ??= new LiveAlertsManager();
    
    private LiveAlertsManager() : base(60)
    {
    }
    
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

    protected override async Task ExecuteTaskAsync()
    {
        var response = await HttpClientHelper.GetAsync<string>(StatusUrl);
        if (!response.Succeeded || response.Content is null)
        {
            // We DONT want to show anything if the request failed.
            // There is no use-case for this and it will only confuse the user.
            StatusMessage = "";
            StatusMessageType = "";
            return;
        }
        
        var (messageType, message) = ParseStatus(response.Content);
        StatusMessage = message;
        StatusMessageType = messageType;
    }
}