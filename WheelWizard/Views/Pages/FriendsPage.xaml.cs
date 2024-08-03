using CT_MKWII_WPF.Models.GameData;
using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Pages;

public partial class FriendsPage : Page, INotifyPropertyChanged
{
    
    private ObservableCollection<Friend> _friendlist = new();
    public ObservableCollection<Friend> FriendList
    {
        get => _friendlist;
        set
        {   
            _friendlist = value;
            OnPropertyChanged(nameof(FriendList));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public FriendsPage()
    {
        InitializeComponent();
        var data = GameDataLoader.Instance;
        data.LoadGameData();
        FriendList = new ObservableCollection<Friend>(data.getCurrentFriends);
        FriendsListView.SortingFunctions.Add("Vr", VrComparable);
        DataContext = this;
        FriendsListView.ItemsSource = FriendList; 
        LoadFirstFriend();

    }

    private void LoadFirstFriend()
    {
        var firstFriend = FriendList.FirstOrDefault();
        if (firstFriend == null) return;
        PlayerStats.UpdateStats(firstFriend);
    }
    
    private static int VrComparable(object x, object y)
    {
        if (x is not Friend xItem || y is not Friend yItem) return 0;
        return xItem.Vr.CompareTo(yItem.Vr);
    }
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CopyFriendCode_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedFriend = FriendsListView.GetCurrentContextItem<Friend>();
        if (selectedFriend == null) return;
        Clipboard.SetText(selectedFriend.FriendCode);
    }


    private void FriendsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var selectedFriend = FriendsListView.SelectedItem as Friend;
        if (selectedFriend == null) return;
        
    }

    private void FriendListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedFriend = FriendsListView.SelectedItem as Friend;
        if (selectedFriend == null) return;
        PlayerStats.UpdateStats(selectedFriend);
    }
}

