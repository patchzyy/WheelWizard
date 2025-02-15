using System;
using System.Collections.Generic;
using System.Linq;
using WheelWizard.Models.MiiImages;
using WheelWizard.Models.RRInfo;
using WheelWizard.Utilities.Mockers.RrInfo;

namespace WheelWizard.Utilities.Mockers;

public class RrRoomFactory : MockingDataFactory<RrRoom, RrRoomFactory>
{
    protected override string DictionaryKeyGenerator(RrRoom value) => value.Id;
    private static int _roomCount = 1;
    private static bool _privatePublicToggle = true;
    public override RrRoom Create()
    {
        var playerCount = (int)(new Random().NextDouble() * 12);
        var players = RrPlayerFactory.Instance.CreateAsDictionary(playerCount); 
        return new RrRoom
        {
            Id = _roomCount++.ToString(),
            Game = "mariokartwii",
            Created = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30)),
            Type = _privatePublicToggle ? "public" : "private",
            Suspend = false,
            Host =  players.First().Value.Name,
            Rk = "vs_10", 
            Players = players
        };
    }
}
