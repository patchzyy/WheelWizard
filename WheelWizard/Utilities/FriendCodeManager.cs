using CT_MKWII_WPF.Models.Enums;
using System.Linq;

namespace CT_MKWII_WPF.Utilities;

public class FriendCodeManager
{
    private static readonly string[] _firstPlaces = new[]
    {
        "3221-2257-0007",
        "1111-1111-1111",
    };

    private static readonly string[] _secondPlaces = new[]
    {
        "1116-6916-3276",
        "1111-1111-1111",
    };

    private static readonly string[] _thirdPlaces = new[]
    {
        "0257-6983-1259",
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
