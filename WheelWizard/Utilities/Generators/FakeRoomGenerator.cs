using System;
using System.Collections.Generic;
using WheelWizard.Models.MiiImages;
using WheelWizard.Models.RRInfo;

namespace WheelWizard.Utilities.Generators;

public static class FakeRoomGenerator
{
    public static RrRoom CreateDesignTestRoom()
    {
        return new RrRoom
        {
            Id = "12345",
            Game = "mariokartwii",
            Created = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30)),
            Type = "public",
            Suspend = false,
            Host = "Player1",
            Rk = "vs_10", 
            Players = new Dictionary<string, RrPlayer>
            {
                {
                    "Player1", new RrPlayer
                    {
                        Count = "1",
                        Pid = "1",
                        Name = "Player 1",
                        ConnMap = "0",
                        ConnFail = "0",
                        Suspend = "0",
                        Fc = "1111-2222-3333",
                        Ev = "9000",
                        Eb = "50",
                        Mii = new List<Mii>
                        {
                            new Mii
                            {
                                Name = "Mii 1",
                                Data = null
                            }
                        }
                    }
                },
                {
                    "Player2", new RrPlayer
                    {
                        Count = "1",
                        Pid = "2",
                        Name = "Player 2",
                        ConnMap = "0",
                        ConnFail = "0",
                        Suspend = "0",
                        Fc = "4444-5555-6666",
                        Ev = "8500",
                        Eb = "60",
                        Mii = new List<Mii>
                        {
                            new Mii
                            {
                                Name = "Mii 2",
                                Data = null
                            }
                        }
                    }
                }
            }
        };
    }
}
