using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.RRInfo;

namespace CT_MKWII_WPF.Services.Networking;

public static class RRLiveRooms
{
    private const string GroupUrl = "http://zplwii.xyz/api/groups";
    private static readonly DispatcherTimer Timer = new() { Interval = TimeSpan.FromSeconds(60) };
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static List<Room> CurrentRooms { get; private set; } = new();
    public static int PlayerCount => CurrentRooms.Sum(room => room.PlayerCount);
    public static int RoomCount => CurrentRooms.Count;
    
    // TODO this is not the way to do it, first of all you dont subscribe actions, but instead classes that implement an interface
    //      anyways, there is also the INotifyPropertyChanged, so that should actually be used after there is a better way that we handle the repeated fetching of the data
    private static readonly List<Action> OnRoomsUpdated = new();
    public static void SubscribeToRoomsUpdated(Action action)
    {
        if (!OnRoomsUpdated.Contains(action))
            OnRoomsUpdated.Add(action);
    }
    public static void UnsubscribeToRoomsUpdated(Action action)
    {
        if (OnRoomsUpdated.Contains(action))
            OnRoomsUpdated.Remove(action);
    }
    
    public static void Start()
    {
        Timer.Tick += async (_, _) => await RefreshRoomsDataAsync();

        Timer.Start();
        Task.Run(RefreshRoomsDataAsync);
    }
    
    public static void Stop() => Timer.Stop();
    
    private static async Task RefreshRoomsDataAsync()
    {
        var response = await HttpClientHelper.GetAsync<List<Room>>(GroupUrl);
        if (!response.Succeeded || response.Content is null)
        {
            // It is not important enough to bore the user with an error message or something.
            // they are not hindered if there is an error, they just dont see the rooms. that's it.
            CurrentRooms = new List<Room>();
            OnRoomsUpdated.ForEach(action => action.Invoke());
            return;
        }

        Console.WriteLine("UPDATGE");
        CurrentRooms = response.Content;
        
        Application.Current.Dispatcher.Invoke(() => OnRoomsUpdated.ForEach(action => action.Invoke()));
    }
}