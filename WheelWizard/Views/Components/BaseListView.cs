using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using static CT_MKWII_WPF.Views.ViewUtils;

namespace CT_MKWII_WPF.Views.Components
{
    public class BaseListView : ListView
    {
        public BaseListView() {
            MouseDoubleClick += ListView_DoubleClick;
        }
        
        protected ListViewItem? _contextListViewItem; // the item that was right-clicked

        public ListViewItem? ContextMenuListViewItem
        {
            get => _contextListViewItem;
            protected set => _contextListViewItem = value;
        }

        public static readonly DependencyProperty ItemContextMenuProperty = DependencyProperty.Register(
            nameof(ItemContextMenu), typeof(ContextMenu), typeof(BaseListView),
            new PropertyMetadata(null));

        public ContextMenu? ItemContextMenu
        {
            get => (ContextMenu)GetValue(ItemContextMenuProperty);
            set => SetValue(ItemContextMenuProperty, value);
        }

        // Define the custom event handlers
        
        public delegate void ItemClickEventHandler(object sender, MouseButtonEventArgs e, ListViewItem clickedItem);
        public delegate void ItemDoubleClickEventHandler(object sender, MouseButtonEventArgs e, ListViewItem clickedItem);
        public event ItemClickEventHandler? OnItemClick;
        public event ItemDoubleClickEventHandler? OnDoubleItemClick;

        protected virtual void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedItem = FindAncestor<ListViewItem>(e.OriginalSource);
            if (clickedItem != null)
                OnItemClick?.Invoke(this, e, clickedItem);
        }
        
        private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var clickedItem = FindAncestor<ListViewItem>(e.OriginalSource);
            if (clickedItem != null)
                OnDoubleItemClick?.Invoke(this, e, clickedItem);
        }

        public T? GetCurrentContextItem<T>() where T : class
        {
            if (_contextListViewItem == null) return null;
            return ItemContainerGenerator.ItemFromContainer(_contextListViewItem) as T;
        }

        protected virtual void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            _contextListViewItem = FindAncestor<ListViewItem>(e.OriginalSource)!;
            if (sender is FrameworkElement && ItemContextMenu != null)
                ItemContextMenu.IsOpen = true;
        }
    }
}
