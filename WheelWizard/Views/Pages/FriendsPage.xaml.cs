using CT_MKWII_WPF.Models.GameData;
using CT_MKWII_WPF.Services.WiiManagement;
using CT_MKWII_WPF.Services.WiiManagement.GameData;
using CT_MKWII_WPF.Utilities.RepeatedTasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CT_MKWII_WPF.Views.Pages;

public partial class FriendsPage : Page, INotifyPropertyChanged
{

    private User _currentUser;

    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            OnPropertyChanged(nameof(CurrentUser));
        }
    }

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
        FriendList = new ObservableCollection<Friend>(data.getCurrentFriends);
        FriendsListView.SortingFunctions.Add("Vr", VrComparable);
        DataContext = this;
        FriendsListView.ItemsSource = FriendList; //for some reason not adding this line
                                                  //causes the listview to not show anything
        PopulatePlayerData();

    }

    private async Task PopulatePlayerData()
    {
        var data = GameDataLoader.Instance;
        PlayerName.Text = data.getCurrentUsername;
        FriendCode.Text = data.getCurrentFriendCode;
        uint vr = data.getCurrentVr;
        VrAndBr.Text = "VR: " + vr;
        MainMii.Source = await MiiImageManager.GetMiiImageAsync(data.getCurrentMiiData.mii.Data);
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

    private void Profile_click(object sender, MouseButtonEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
    {
        Cursor = Cursors.Hand;
    }

    private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
    {
        Cursor = Cursors.Arrow;
    }
}

