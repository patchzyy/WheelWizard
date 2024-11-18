using System.Linq;
using WheelWizard.Models.Enums;

namespace WheelWizard.Utilities;

public class FriendCodeManager
{
    private static readonly string[] _firstPlaces = new[]
    {
        "4343-3434-3434",
        "2277-7727-2227"
    };

    private static readonly string[] _secondPlaces = new[]
    {
        "1251-5622-1012",
        "0000-0202-1121"
    };

    private static readonly string[] _thirdPlaces = new[]
    {
        "3955-9063-2091",
        "4988-1656-7319"
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
