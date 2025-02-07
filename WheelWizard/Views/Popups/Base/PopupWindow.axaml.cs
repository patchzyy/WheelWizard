using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.ComponentModel;
using WheelWizard.Services.Settings;

namespace WheelWizard.Views.Popups;

public partial class PopupWindow : Window, INotifyPropertyChanged
{
    public PopupWindow()
    {
        // Constructor is never used, however, UI elements must have a constructor with no params
        InitializeComponent();
        DataContext = this;
        Loaded += PopupWindow_Loaded;
    }
    
   private static int _disableCount = 0; // This is used to keep track of how many popups are open that disable the layout
    // We only want to re-enable the layout when all popups that are disabling it are closed
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
    public PopupWindow(bool allowClose, bool allowLayoutInteraction, bool isTopMost, string title = "", Vector? size = null)
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
        Width = size!.Value.X * scaleFactor;
        Height = size.Value.Y * scaleFactor;
        CompleteGrid.RenderTransform = new ScaleTransform(scaleFactor, scaleFactor);
        var marginXCorrection = ((scaleFactor * size.Value.X) - size.Value.X)/2f;
        var marginYCorrection = ((scaleFactor * size.Value.Y) - size.Value.Y)/2f;
        CompleteGrid.Margin = new Thickness(marginXCorrection, marginYCorrection);
        Position = mainWindow.Position;
        Loaded += PopupWindow_Loaded;
    }
    

    private void PopupWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        BeforeOpen();
        if (_allowLayoutInteraction) return;

        ViewUtils.GetLayout().DisableEverything();
        _disableCount++;
    }
    
    private void TopBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    
    protected override void OnClosed(EventArgs e)
    {
        BeforeClose();
        if (!_allowLayoutInteraction)
        {
            _disableCount--;
            if (_disableCount <= 0)
                ViewUtils.GetLayout().EnableEverything();
        }
        base.OnClosed(e);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

