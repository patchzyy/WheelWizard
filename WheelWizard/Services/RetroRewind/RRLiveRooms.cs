using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Utilities;
using CT_MKWII_WPF.Utilities.RepeatedTasks;

namespace CT_MKWII_WPF.Services.RetroRewind;

public class RRLiveRooms : RepeatedTaskManager
{
    private const string GroupUrl = "http://zplwii.xyz/api/groups";
    public List<Room> CurrentRooms { get; private set; } = new();
    public int PlayerCount => CurrentRooms.Sum(room => room.PlayerCount);
    public int RoomCount => CurrentRooms.Count;

    private static RRLiveRooms? _instance;
    public static RRLiveRooms Instance => _instance ??= new RRLiveRooms();

    private RRLiveRooms() : base(10)
    {
    }

    protected override async Task ExecuteTaskAsync()
    {
        //Console.WriteLine("reset rooms");
        var response = await HttpClientHelper.GetAsync<List<Room>>(GroupUrl);
        //var response = HttpClientHelper.MockResult<List<Room>>(Mocker.GroupApiResponse);
            
        // It is not important enough to bore the user with an error message or something.
        // they are not hindered if there is an error, they just dont see the rooms. that's it.
        if (!response.Succeeded || response.Content is null)
            CurrentRooms = new List<Room>();
        else CurrentRooms = response.Content;
    }
}