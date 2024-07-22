using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Pages.KitchenSink;

public partial class KsLists : UserControl, INotifyPropertyChanged
{
    private readonly ObservableCollection<Person> _personList = new();
    public ObservableCollection<Person> PersonList
    {
        get => _personList;
        init
        {
            _personList = value;
            OnPropertyChanged(nameof(PersonList));
        }
    }

    public KsLists()
    {
        InitializeComponent();
        DataContext = this;
        
        PersonList.Add(new Person { Name = "John", Age = 25 });
        PersonList.Add(new Person { Name = "Jane", Age = 30 });
        PersonList.Add(new Person { Name = "Joe", Age = 35 });
    }
    
    private void ChangeHelperText(string text) => HelperText.Text = text;
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void DraggableListView_OnOnItemsReorder(ListViewItem movedItem, int newIndex)
    {
        var selectedPerson = (Person)movedItem.DataContext;
        ChangeHelperText($"Moved {selectedPerson.Name}-{selectedPerson.Age} to index {newIndex}");
    }
    
    private void ListView_OnOnItemClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
    {
        var selectedPerson = (Person)clickedItem.DataContext;
        ChangeHelperText($"Clicked {selectedPerson.Name}-{selectedPerson.Age}");
    }
    
    private void ListView_OnOnItemDoubleClick(object sender, MouseButtonEventArgs e, ListViewItem clickedItem)
    {
        var selectedPerson = (Person)clickedItem.DataContext;
        ChangeHelperText($"Double Clicked {selectedPerson.Name}-{selectedPerson.Age}!");
    }
    
    private void MenuItem_OnClick1(object sender, RoutedEventArgs e)
    {
        // Note that for the context menu, You need a reference (x:Name) to the original ListView in order to get the actual item you clicked on
        var selectedPerson = BottomList.GetCurrentContextItem<Person>();
        if (selectedPerson == null) return;
        ChangeHelperText($"Context click 1 {selectedPerson.Name}-{selectedPerson.Age}!");
    }
    
    private void MenuItem_OnClick2(object sender, RoutedEventArgs e)
    {
        // Note that for the context menu, You need a reference (x:Name) to the original ListView in order to get the actual item you clicked on
        var selectedPerson = BottomList.GetCurrentContextItem<Person>();
        if (selectedPerson == null) return;
        ChangeHelperText($"Context click 2 {selectedPerson.Name}-{selectedPerson.Age}!");
    }
    
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }   
    }
}
