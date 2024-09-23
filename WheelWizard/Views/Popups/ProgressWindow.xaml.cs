using CT_MKWII_WPF.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CT_MKWII_WPF.Views.Popups;

public partial class ProgressWindow : PopupContent
{
    
    // WindowTitle = what this means for the user (e.g: "updating retro rewind")
    // goalText    = what this means for the computer (e.g: "downloading 23 files (5.4 MB)")
    // extraText   = A little description of what is happening (e.g: "This may take a while" or "placing files in the mods folder")
    // progress    = i hope this is straight forward lol
    private Stopwatch _stopwatch = new();
    private int _progress = 0;
    private double? _totalMb = null;
    
    public ProgressWindow(string windowTitle) : base(false, false, windowTitle, new(400,230))
    {
        InitializeComponent();
    }

    protected override void BeforeOpen()
    {
        _stopwatch.Start();
    }

    protected override void BeforeClose()
    {
        _stopwatch.Stop();
    }

    private void InternalUpdate()
    {
        var elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;
        var remainingSeconds = (100 - _progress) / (_progress / elapsedSeconds);
        var remainingText = Humanizer.HumanizeSeconds((int)remainingSeconds);
        if (_progress <= 0)
            remainingText = "unknown";
        var bottomText = $"Estimated time remaining: { remainingText }";
        if (_totalMb != null)
        {
            var downloadedMb = (_progress / 100.0) * (double)_totalMb;
            bottomText = $"Speed: {downloadedMb / elapsedSeconds:F2} MB/s | {bottomText}";
        }
        
        LiveTextBlock.Text = bottomText;
        ProgressBar.Value = _progress;
    }
    
    public ProgressWindow SetExtraText(string mainText)
    {
        ExtraTextBlock.Text = mainText;
        return this;
    }
    
    public ProgressWindow SetGoal(string extraText, double? megaBytes = null)
    {
        _totalMb = megaBytes;
        if (megaBytes == null)
            GoalTextBlock.Text = extraText;
        else
            GoalTextBlock.Text = extraText + $" ({megaBytes:F2} MB)";
        
        return this;
    }
    public ProgressWindow SetGoal(double megaBytes)
    {
        _totalMb = megaBytes;
        GoalTextBlock.Text = $"downloading {megaBytes:F2} MB";
        return this;
    }

    public void UpdateProgress(int progress)
    {
        _progress = progress;
        InternalUpdate();
    }
}
