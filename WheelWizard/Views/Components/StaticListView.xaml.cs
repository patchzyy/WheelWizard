using System.Windows.Controls;
using System.Windows;

namespace CT_MKWII_WPF.Views.Components;


public partial class StaticListView : BaseListView
{
    public StaticListView() : base()
    {
        InitializeComponent();
        FontSize = 14.0;
    }
    
    public static readonly DependencyProperty IsClickableProperty = DependencyProperty.Register(
        nameof(IsClickable), typeof(bool), typeof(BaseListView),
        new PropertyMetadata(true));

    public bool IsClickable
    {
        get => (bool)GetValue(IsClickableProperty);
        set => SetValue(IsClickableProperty, value);
    }
}
