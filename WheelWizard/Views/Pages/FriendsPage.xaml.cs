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
        var currentUser = data.GameData.Users[3];
        MessageBox.Show(currentUser.MiiData.mii.Name);
        MessageBox.Show(currentUser.Friends.Count.ToString());
        CurrentUser = currentUser;
        FriendList = new ObservableCollection<Friend>(CurrentUser.Friends);
        FriendsListView.SortingFunctions.Add("Vr", VrComparable);
        DataContext = this;
        FriendsListView.ItemsSource = FriendList; //for some reason not adding this line
                                                  //causes the listview to not show anything
        PopulatePlayerData();

    }

    private async Task PopulatePlayerData()
    {
        var data = GameDataLoader.Instance;
        PlayerName.Text = data.GameData.Users[3].MiiData.mii.Name; 
        FriendCode.Text = data.GameData.Users[3].FriendCode;
        uint vr = data.GameData.Users[3].Vr;
        uint br = data.GameData.Users[3].Br;
        VrAndBr.Text = "VR: " + vr;
        MainMii.Source = await MiiImageManager.GetMiiImageAsync(data.GameData.Users[3].MiiData.mii.Data);
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
}

