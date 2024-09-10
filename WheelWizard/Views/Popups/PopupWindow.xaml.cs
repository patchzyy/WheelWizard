using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Popups;

public partial class PopupWindow : Window, INotifyPropertyChanged
{
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

    private readonly bool _allowLayoutInteraction;
    
    public PopupWindow(bool allowClose = true, bool allowLayoutInteraction = false)
    {
        CanClose = allowClose;
        _allowLayoutInteraction = allowLayoutInteraction;
        var mainWindow = ViewUtils.GetLayout();
        if(mainWindow.IsVisible)
            Owner = mainWindow;
        
        InitializeComponent();
        DataContext = this;
        Loaded += PopupWindow_Loaded;
    }
    
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    
    private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_allowLayoutInteraction)
            ViewUtils.GetLayout().DisableEverything();
    }
    
    protected override void OnClosed(EventArgs e)
    {
        ViewUtils.GetLayout().EnableEverything();
        base.OnClosed(e);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
