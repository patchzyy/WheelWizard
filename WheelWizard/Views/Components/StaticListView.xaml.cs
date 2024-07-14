using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace CT_MKWII_WPF.Views.Components;


public partial class StaticListView : BaseListView
{
    private GridViewColumnHeader? _lastHeaderClicked;
    private ListSortDirection _lastDirection = ListSortDirection.Ascending;

    public StaticListView() : base()
    {
        InitializeComponent();
        FontSize = 14.0;
        AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewColumnHeader_Click));
    }
    
    public static readonly DependencyProperty IsClickableProperty = DependencyProperty.Register(
        nameof(IsClickable), typeof(bool), typeof(StaticListView),
        new PropertyMetadata(true));

    public bool IsClickable
    {
        get => (bool)GetValue(IsClickableProperty);
        set => SetValue(IsClickableProperty, value);
    }
    
    public static readonly DependencyProperty ListTitleProperty = DependencyProperty.Register(
        nameof(ListTitle), typeof(string), typeof(StaticListView),
        new PropertyMetadata(string.Empty));

    public string ListTitle
    {
        get => (string)GetValue(ListTitleProperty);
        set => SetValue(ListTitleProperty, value);
    }
    
    public Dictionary<string, Func<object?, object?, int>> SortingFunctions { get; set; } = new();
    
    public static readonly
        DependencyProperty IsSortableProperty = DependencyProperty.RegisterAttached(
        "IsSortable", typeof(bool), typeof(StaticListView), new PropertyMetadata(false));

    public static void SetIsSortable(DependencyObject element, bool value) => element.SetValue(IsSortableProperty, value);
    public static bool GetIsSortable(DependencyObject element) => (bool)element.GetValue(IsSortableProperty);
    
    private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not GridViewColumnHeader headerClicked || headerClicked.Role == GridViewColumnHeaderRole.Padding) return;
        
        if (!GetIsSortable(headerClicked.Column)) return;

        ListSortDirection direction;

        if (headerClicked != _lastHeaderClicked)
            direction = ListSortDirection.Ascending;
        else
        {
            direction = _lastDirection == ListSortDirection.Ascending ? 
                ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
        var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;
        if (sortBy != null) 
            Sort(sortBy, direction);

        if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
            SetSortArrow(_lastHeaderClicked, null);

        SetSortArrow(headerClicked, direction);

        _lastHeaderClicked = headerClicked;
        _lastDirection = direction;
    }

    private static void SetSortArrow(GridViewColumnHeader header, ListSortDirection? direction)
    {
        if (header.Template.FindName("SortArrow", header) is not PackIconFontAwesome sortArrow) return;
        
        switch (direction)
        {
            case ListSortDirection.Ascending:
                sortArrow.Visibility = Visibility.Visible;
                sortArrow.Kind = PackIconFontAwesomeKind.ChevronUpSolid;
                break;
            case ListSortDirection.Descending:
                sortArrow.Visibility = Visibility.Visible;
                sortArrow.Kind = PackIconFontAwesomeKind.ChevronDownSolid;
                break;
            default:
                sortArrow.Visibility = Visibility.Collapsed;
                break;
        }
    }
    
    private void Sort(string sortBy, ListSortDirection direction)
    {
        var dataView = CollectionViewSource.GetDefaultView(ItemsSource) as ListCollectionView;
        if (dataView == null) return;
        
        dataView.SortDescriptions.Clear();
        var sd = new SortDescription(sortBy, direction);
        dataView.SortDescriptions.Add(sd);
        
        if (SortingFunctions.TryGetValue(sortBy, out var customComparer)) 
            dataView.CustomSort = new LambdaComparer(customComparer, direction);
        
        dataView.Refresh();
    }

    private void GridViewColumnHeader_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is not Border { TemplatedParent: GridViewColumnHeader header }) return;
        var column = header.Column;
        if (column == null) return;
        if (!GetIsSortable(column)) return;

        header.Cursor = Cursors.Hand;
        header.Foreground = Application.Current.FindResource("StaticItemForeground-Hover+") as SolidColorBrush;
    }
    private void GridViewColumnHeader_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is not Border { TemplatedParent: GridViewColumnHeader header }) return;
        var column = header.Column;
        if (column == null) return;
        if (!GetIsSortable(column)) return;
        
        header.Cursor = Cursors.Arrow;
        header.Foreground = Application.Current.FindResource("HeaderAttributes") as SolidColorBrush;
    }
    
    public class LambdaComparer : IComparer
    {
        private readonly Func<object?, object?, int> _comparer;
        private readonly ListSortDirection _direction;

        public LambdaComparer(Func<object?, object?, int> comparer, ListSortDirection direction)
        {
            _comparer = comparer;
            _direction = direction;
        }

        public int Compare(object? x, object? y)
        {
            var result = _comparer(x, y);
            if (_direction == ListSortDirection.Descending) result = -result;
            return result;
        }
    }
}
