using System;

namespace CT_MKWII_WPF.Helpers;

public static class Humanizer
{
    public static string HumanizeTimeSpan(TimeSpan timeSpan)
    {
        if (Math.Abs(timeSpan.TotalDays) >= 1)
            return $"{timeSpan.Days} day{P(timeSpan.Days)} {Math.Abs(timeSpan.Hours)} hour{P(timeSpan.Hours)}";
        if (Math.Abs(timeSpan.TotalHours) >= 1)
            return $"{timeSpan.Hours} hour{P(timeSpan.Hours)} {Math.Abs(timeSpan.Minutes)} minute{P(timeSpan.Minutes)}";
        if (Math.Abs(timeSpan.TotalMinutes) >= 1)
            return $"{timeSpan.Minutes} minute{P(timeSpan.Minutes)} {Math.Abs(timeSpan.Seconds)} second{P(timeSpan.Seconds)}";

        return $"{timeSpan.Seconds} second{P(timeSpan.Seconds)}";

        // internal method to simplify the pluralization of words
        string P(int count) => Math.Abs(count) != 1 ? "s" : "";
    }
    
    public static string GetRegionName(uint regionID)
    {
        switch (regionID)
        {
            case 0:
                return "Japan";
            case 1:
                return "America";
            case 2:
                return "Europe";
            case 3:
                return "Australia";
            case 4:
                return "Taiwan";
            case 5:
                return "South Korea";
            case 6:
                return "China";
            default:
                return "Unknown";
        }
    }
    
    public static string GetCountryEmoji(byte countryId)
{
    switch (countryId)
    {
        // Japan
        case 1:
            return "🇯🇵";

        // Americas
        case 8:
            return "🇦🇮"; // Anguilla
        case 9:
            return "🇦🇬"; // Antigua and Barbuda
        case 10:
            return "🇦🇷"; // Argentina
        case 11:
            return "🇦🇲"; // Aruba
        case 12:
            return "🇧🇸"; // Bahamas
        case 13:
            return "🇧🇧"; // Barbados
        case 14:
            return "🇧🇿"; // Belize
        case 15:
            return "🇧🇴"; // Bolivia
        case 16:
            return "🇧🇷"; // Brazil
        case 17:
            return "🇻🇬"; // British Virgin Islands
        case 18:
            return "🇨🇦"; // Canada
        case 19:
            return "🇰🇾"; // Cayman Islands
        case 20:
            return "🇨🇱"; // Chile
        case 21:
            return "🇨🇴"; // Colombia
        case 22:
            return "🇨🇷"; // Costa Rica
        case 23:
            return "🇩🇲"; // Dominica
        case 24:
            return "🇩🇴"; // Dominican Republic
        case 25:
            return "🇪🇨"; // Ecuador
        case 26:
            return "🇸🇻"; // El Salvador
        case 27:
            return "🇫🇷"; // French Guiana
        case 28:
            return "🇬🇩"; // Grenada
        case 29:
            return "🇲🇶"; // Guadeloupe
        case 30:
            return "🇵🇪"; // Guatemala
        case 31:
            return "🇬🇾"; // Guyana
        case 32:
            return "🇭🇹"; // Haiti
        case 33:
            return "🇭🇳"; // Honduras
        case 34:
            return "🇯🇲"; // Jamaica
        case 35:
            return "🇲🇶"; // Martinique
        case 36:
            return "🇲🇽"; // Mexico
        case 37:
            return "🇲🇸"; // Montserrat
        case 38:
            return "🇳🇱"; // Netherlands Antilles
        case 39:
            return "🇳🇮"; // Nicaragua
        case 40:
            return "🇵🇦"; // Panama
        case 41:
            return "🇵🇾"; // Paraguay
        case 42:
            return "🇵🇪"; // Peru
        case 43:
            return "🇰🇳"; // St. Kitts and Nevis
        case 44:
            return "🇱🇨"; // St. Lucia
        case 45:
            return "🇻🇨"; // St. Vincent and the Grenadines
        case 46:
            return "🇸🇷"; // Suriname
        case 47:
            return "🇹🇹"; // Trinidad and Tobago
        case 48:
            return "🇹🇨"; // Turks and Caicos Islands
        case 49:
            return "🇺🇸"; // United States
        case 50:
            return "🇺🇾"; // Uruguay
        case 51:
            return "🇻🇮"; // US Virgin Islands
        case 52:
            return "🇻🇪"; // Venezuela

        // Europe & Africa
        case 64:
            return "🇦🇱"; // Albania
        case 65:
            return "🇦🇺"; // Australia
        case 66:
            return "🇦🇹"; // Austria
        case 67:
            return "🇧🇪"; // Belgium
        case 68:
            return "🇧🇦"; // Bosnia and Herzegovina
        case 69:
            return "🇧🇼"; // Botswana
        case 70:
            return "🇧🇬"; // Bulgaria
        case 71:
            return "🇭🇷"; // Croatia
        case 72:
            return "🇨🇾"; // Cyprus
        case 73:
            return "🇨🇿"; // Czech Republic
        case 74:
            return "🇩🇰"; // Denmark
        case 75:
            return "🇪🇪"; // Estonia
        case 76:
            return "🇫🇮"; // Finland
        case 77:
            return "🇫🇷"; // France
        case 78:
            return "🇩🇪"; // Germany
        case 79:
            return "🇬🇷"; // Greece
        case 80:
            return "🇭🇺"; // Hungary
        case 81:
            return "🇮🇸"; // Iceland
        case 82:
            return "🇮🇪"; // Ireland
        case 83:
            return "🇮🇹"; // Italy
        case 84:
            return "🇱🇻"; // Latvia
        case 85:
            return "🇱🇸"; // Lesotho
        case 86:
            return "🇱🇮"; // Liechtenstein
        case 87:
            return "🇱🇹"; // Lithuania
        case 88:
            return "🇱🇺"; // Luxembourg
        case 89:
            return "🇲🇰"; // North Macedonia
        case 90:
            return "🇲🇹"; // Malta
        case 91:
            return "🇲🇪"; // Montenegro
        case 92:
            return "🇲🇿"; // Mozambique
        case 93:
            return "🇳🇦"; // Namibia
        case 94:
            return "🇳🇱"; // Netherlands
        case 95:
            return "🇳🇿"; // New Zealand
        case 96:
            return "🇳🇴"; // Norway
        case 97:
            return "🇵🇱"; // Poland
        case 98:
            return "🇵🇹"; // Portugal
        case 99:
            return "🇷🇴"; // Romania
        case 100:
            return "🇷🇺"; // Russia
        case 101:
            return "🇷🇸"; // Serbia
        case 102:
            return "🇸🇰"; // Slovakia
        case 103:
            return "🇸🇮"; // Slovenia
        case 104:
            return "🇿🇦"; // South Africa
        case 105:
            return "🇪🇸"; // Spain
        case 106:
            return "🇸🇿"; // Eswatini
        case 107:
            return "🇸🇪"; // Sweden
        case 108:
            return "🇨🇭"; // Switzerland
        case 109:
            return "🇹🇷"; // Turkey
        case 110:
            return "🇬🇧"; // United Kingdom
        case 111:
            return "🇿🇲"; // Zambia
        case 112:
            return "🇿🇼"; // Zimbabwe
        case 113:
            return "🇦🇿"; // Azerbaijan
        case 114:
            return "🇲🇷"; // Mauritania
        case 115:
            return "🇲🇱"; // Mali
        case 116:
            return "🇳🇪"; // Niger
        case 117:
            return "🇹🇩"; // Chad
        case 118:
            return "🇸🇩"; // Sudan
        case 119:
            return "🇪🇷"; // Eritrea
        case 120:
            return "🇩🇯"; // Djibouti
        case 121:
            return "🇸🇴"; // Somalia

        // Southeast Asia
        case 128:
            return "🇹🇼"; // Taiwan
        case 136:
            return "🇰🇷"; // South Korea
        case 144:
            return "🇭🇰"; // Hong Kong
        case 145:
            return "🇲🇴"; // Macao
        case 152:
            return "🇮🇩"; // Indonesia
        case 153:
            return "🇸🇬"; // Singapore
        case 154:
            return "🇹🇭"; // Thailand
        case 155:
            return "🇵🇭"; // Philippines
        case 156:
            return "🇲🇾"; // Malaysia
        case 160:
            return "🇨🇳"; // China

        // Middle East
        case 168:
            return "🇦🇪"; // U.A.E.
        case 169:
            return "🇮🇳"; // India
        case 170:
            return "🇪🇬"; // Egypt
        case 171:
            return "🇴🇲"; // Oman
        case 172:
            return "🇶🇦"; // Qatar
        case 173:
            return "🇰🇼"; // Kuwait
        case 174:
            return "🇸🇦"; // Saudi Arabia
        case 175:
            return "🇸🇾"; // Syria
        case 176:
            return "🇧🇭"; // Bahrain
        case 177:
            return "🇯🇴"; // Jordan

        default:
            return "🏳️"; // Unknown flag emoji
    }
}
}
