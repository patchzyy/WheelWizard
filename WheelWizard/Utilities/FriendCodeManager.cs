using System.Linq;
using WheelWizard.Models.Enums;

namespace WheelWizard.Utilities;

public class FriendCodeManager
{
    private static readonly string[] _firstPlaces = new[]
    {
        "1111-1111-1111",
    };

    private static readonly string[] _secondPlaces = new[]
    {
        "1111-1111-1111",
    };

    private static readonly string[] _thirdPlaces = new[]
    {
        "1111-1111-1111",
    };

    public static PlayerWinPosition GetWinPosition(string friendCode)
    {
        if (_firstPlaces.Contains(friendCode))
            return PlayerWinPosition.First;
        if (_secondPlaces.Contains(friendCode))
            return PlayerWinPosition.Second;
        if (_thirdPlaces.Contains(friendCode))
            return PlayerWinPosition.Third;
        
        return PlayerWinPosition.None;
    }
}
