using WheelWizard.Resources.Languages;
using WheelWizard.Utilities;
using System;
using System.Windows;
using System.Xml.Linq;
using WheelWizard.Helpers;
using WheelWizard.Services.LiveData;
using WheelWizard.Services.WiiManagement;
using WheelWizard.Utilities.DevTools;
using WheelWizard.Utilities.RepeatedTasks;

namespace WheelWizard.Views.Popups;

public partial class DevToolWindow : PopupContent, IRepeatedTaskListener
{
    
    // Close is allowed, since its the exact same as pressing no
    public DevToolWindow() : base(true, true,false ,"Dev Tool", new(400, 600))
    {
        InitializeComponent();
        AppStateMonitor.Instance.Subscribe(this);
        LoadSettings();
    }
    
    protected override void BeforeClose()
    {
        AppStateMonitor.Instance.Unsubscribe(this);
        base.BeforeClose();
    }

    // Yes, it would absolutely be more optimized to insteadof every x seconds refreshing, to just refresh when something changes
    // However, We explicitly do it this way, so all code in the codebase can stay unchanged. The idea is that you can remove the AppStateMonitor and everything will still work
    // This is indeed also possible if you make it if you make everything an observer pattern, where-ever you want to monitor something.
    // However, the problem with that is that we will everything an observer pattern, but something like the MiiImageManager has no reason
    // to be an observer pattern besides this, and it would make the codebase more complex for no reason.
    public void OnUpdate(RepeatedTaskManager sender)
    {
    
        RrRefreshTimeLeft.Text = RRLiveRooms.Instance.TimeUntilNextTick.Seconds.ToString();
        MiiImagesCashed.Text = MiiImageManager.ImageCount.ToString();
    }

    private void LoadSettings()
    {
        WhWzTopMost.IsChecked = ViewUtils.GetLayout().Topmost;
        HttpHelperOff.IsChecked = !HttpClientHelper.FakeConnectionToInternet;
    }
    
    private void WhWzTopMost_OnClick(object sender, RoutedEventArgs e) => ViewUtils.GetLayout().Topmost = WhWzTopMost.IsChecked == true;
    private void HttpHelperOff_OnClick(object sender, RoutedEventArgs e) => HttpClientHelper.FakeConnectionToInternet = HttpHelperOff.IsChecked != true;
    private void ForceEnableLayout_OnClick(object sender, RoutedEventArgs e) => ViewUtils.GetLayout().EnableEverything();
}
