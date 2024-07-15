using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static CT_MKWII_WPF.Views.ViewUtils;

namespace CT_MKWII_WPF.Views.Components;

public partial class DraggableListView : BaseListView
{
    private ListViewItem? _draggingListViewItem; // the item that is being dragged
    private ListViewItem? _dragHoverListViewItem; // the item that is being hovered over while dragging an other item
    private bool? _previousBotSide;
    private Point? _startDraggingPoint;

    public DraggableListView() : base()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        DragOver += DraggableListView_DragOver;
        AddHandler(DragDrop.QueryContinueDragEvent, new QueryContinueDragEventHandler(OnQueryContinueDrag));
    }
    
    public static readonly DependencyProperty EnableLineIndicationProperty = DependencyProperty.Register(
        nameof(EnableLineIndication), typeof(bool), typeof(DraggableListView),
        new PropertyMetadata(true));

    public bool EnableLineIndication
    {
        get => (bool)GetValue(EnableLineIndicationProperty);
        set => SetValue(EnableLineIndicationProperty, value);
    }
    
     public delegate void ItemsReorderEventHandler(ListViewItem movedItem, int newIndex);
     public event ItemsReorderEventHandler? OnItemsReorder;
        
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (View is not GridView gridView) return; 

        // here we create an empty column that will later be filled with the icon for each row
        // We create an empty column that will be filled in order to not mess up any other columns arrangement
        var gripColumn = new GridViewColumn {
            Width = 40,
            CellTemplate = new DataTemplate(),
            Header = new GridViewColumnHeader( )
        };
        
        gridView.Columns.Insert(0, gripColumn);
    }

    private void GripIcon_Hold(object sender, MouseButtonEventArgs e)
    {
        _draggingListViewItem = FindAncestor<ListViewItem>(e.OriginalSource)!;
    
        var height = _draggingListViewItem.ActualHeight;
        _draggingListViewItem.Style = (Style)FindResource("DraggedItemStyle");
        _draggingListViewItem.Height = height; 
        _startDraggingPoint = e.GetPosition(this);
        
        var dragData = new DataObject("listViewItem", _draggingListViewItem);
        DragDrop.DoDragDrop(_draggingListViewItem, dragData, DragDropEffects.Move);
    }

    private void CancelDropAction()
    {
        if (_draggingListViewItem != null)
            _draggingListViewItem.Style = (Style)FindResource("DefaultItemStyle");
        if (_dragHoverListViewItem != null)
            _dragHoverListViewItem.Style = (Style)FindResource("DefaultItemStyle");
        _draggingListViewItem = null;
        _dragHoverListViewItem = null;
        _previousBotSide = null;
        _startDraggingPoint = null;
    }
    
    private void GripIcon_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) CancelDropAction();
    }
    
    private void DraggableListView_DragOver(object sender, DragEventArgs e) => UpdateDragHoverItem(e);
    private void UpdateDragHoverItem(DragEventArgs e)
    {
        var targetData = GetDropTargetIndex(e.GetPosition(this));
        var viewItem = ItemContainerGenerator.ContainerFromIndex(targetData.Item1) as ListViewItem;
       
        // TODO: make it so it does not change the style if the smae targetData has been given,
        //       this will make it so it does not flicker
        if (!EnableLineIndication || viewItem == null || (viewItem == _dragHoverListViewItem)) return;
        if (_dragHoverListViewItem != null)
            _dragHoverListViewItem.Style = (Style)FindResource("DefaultItemStyle");
        if (viewItem != _draggingListViewItem)
        {
            _dragHoverListViewItem = viewItem;
            var side = targetData.Item2 ? "Down" : "Up";
            _previousBotSide = targetData.Item2;
            _dragHoverListViewItem.Style = (Style)FindResource($"DefaultItemStyle-Target{side}");
        }
        else
        {
            _dragHoverListViewItem = null;
            _previousBotSide = null;
        }
    }
    
    private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        if (e.EscapePressed || e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
            CancelDropAction();
    }
    
    private void GripIcon_OnDrop(object sender, DragEventArgs e)
    {
        if(!(e.Data.GetData("listViewItem") is ListViewItem listViewItem)) return;
        if(_draggingListViewItem == null) return; // this should never happen, so to be safe we just return
        var targetData = GetDropTargetIndex(e.GetPosition(this));
        var targetIndex = targetData.Item1;
        if (targetData.Item2) targetIndex++;

        // Getting a bunch of types to call the Remove and Insert methods
        var itemObject = ItemContainerGenerator.ItemFromContainer(listViewItem)!;
        var itemIndex = ItemContainerGenerator.IndexFromContainer(listViewItem);
        if (itemIndex < targetIndex) targetIndex--;
        var itemType = itemObject.GetType();
        var genericCollectionType = typeof(ObservableCollection<>).MakeGenericType(itemType);
        var removeMethod = genericCollectionType.GetMethod("Remove", new[] { itemType });
        var insertMethod = genericCollectionType.GetMethod("Insert", new[] { typeof(int), itemType });
        
        OnItemsReorder?.Invoke(_draggingListViewItem, targetIndex);
        if (removeMethod != null && insertMethod != null)
        {
            _draggingListViewItem = null;
            removeMethod.Invoke(ItemsSource, new[] { itemObject });
            insertMethod.Invoke(ItemsSource, new object[] { targetIndex , itemObject });
        } 
        else Console.WriteLine("It seems this collection type does not support in-place reordering");

        _startDraggingPoint = null;
        if (_dragHoverListViewItem != null)
            _dragHoverListViewItem.Style = (Style)FindResource("DefaultItemStyle");
    }
    
    private IEnumerable<ListViewItem> GetAllListViewItems()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is ListViewItem container) yield return container;
            // else
            // {
            //     // If the container is null, it might not be realized yet
            //     ScrollIntoView(Items[i]);
            //     container = ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
            //     if (container != null) yield return container;
            // }
        }
    }
    
    // This method returns 2 indexes.
    // Item1 = the target index where it should place
    // Item2 = wether or not it should be displayed as bottom. (if so, Item1 also has to be decreased by 1)
    private Tuple<int,bool> GetDropTargetIndex(Point dropPosition)
    {
        var index = 0;
        foreach (var listViewItem in GetAllListViewItems())
        {
            Rect itemBounds = VisualTreeHelper.GetDescendantBounds(listViewItem);
            Point itemPosition = listViewItem.TransformToAncestor(this).Transform(new Point(0, 0));
            double itemBottom = itemPosition.Y + itemBounds.Height;

            var showBottom = false;
            if(_startDraggingPoint != null)
                showBottom = dropPosition.Y > _startDraggingPoint?.Y;
            
            if (dropPosition.Y < itemBottom) return new Tuple<int, bool>(index, showBottom);
            index++;
        }
        
        return new Tuple<int,bool>(Items.Count - 1, true);
    }
}
