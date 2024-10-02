using CT_MKWII_WPF.Services.Settings;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Popups;

public partial class PopupWindow : Window, INotifyPropertyChanged
{
    private bool _isTopMost = true;
    
    public bool IsTopMost
    {
        get => _isTopMost;
        set
        {
            _isTopMost = value;
            Topmost = value;
            OnPropertyChanged(nameof(IsTopMost));
        }
    }
    
    private bool _canClose = false;
    public bool CanClose
    {
        get => _canClose;
        set
        {   
            _canClose = value;
            OnPropertyChanged(nameof(CanClose));
        }
    }
    
    private string _windowTitle = "Wheel Wizard Popup";
    public string WindowTitle
    {
        get => _windowTitle;
        set
        {   
            _windowTitle = value;
            OnPropertyChanged(nameof(WindowTitle));
        }
    }

    public Action BeforeOpen { get; set; } = () => { };
    public Action BeforeClose { get; set; } = () => { };
    
    private readonly bool _allowLayoutInteraction;
    
    // Most (if not all) of these parameters should be set in the popup you create, and not kept as a parameter for that popup
    public PopupWindow(bool allowClose, bool allowLayoutInteraction,bool isTopMost, string title = "", Vector? size = null)
    {
        size ??= new(400, 200);
        IsTopMost = isTopMost;
        CanClose = allowClose;
        WindowTitle = title;
        _allowLayoutInteraction = allowLayoutInteraction;
        var mainWindow = ViewUtils.GetLayout();
        if(mainWindow.IsVisible)
            Owner = mainWindow;
        
        InitializeComponent();
        DataContext = this;
                
        var scaleFactor = (double)SettingsManager.WINDOW_SCALE.Get();
        ScaleTransform.ScaleX = scaleFactor;
        ScaleTransform.ScaleY = scaleFactor;
        Width = size!.Value.X * scaleFactor;
        Height = size.Value.Y * scaleFactor;
        
        Loaded += PopupWindow_Loaded;
    }
    
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    
    private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
    {
        BeforeOpen();
        if (!_allowLayoutInteraction)
            ViewUtils.GetLayout().DisableEverything();
    }
    
    protected override void OnClosed(EventArgs e)
    {
        BeforeClose();
        ViewUtils.GetLayout().EnableEverything();
        base.OnClosed(e);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
