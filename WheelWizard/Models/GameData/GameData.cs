using WheelWizard.Services.Settings;
using System.Collections.Generic;

namespace WheelWizard.Models.GameData;

public class GameData
{
    public List<User> Users { get; set; }

    public GameData()
    {
        Users = new List<User>(4);
    }
}
