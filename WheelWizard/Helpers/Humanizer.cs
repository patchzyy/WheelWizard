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
        return regionID switch
        {
            0 => "Japan",
            1 => "America",
            2 => "Europe",
            3 => "Australia",
            4 => "Taiwan",
            5 => "South Korea",
            6 => "China",
            _ => "Unknown"
        };
    }
    
    public static string GetCountryEmoji(byte countryId)
    {
        return countryId switch
        {
            // Japan
            1 => "🇯🇵",

            // Americas
            8 => "🇦🇮", // Anguilla
            9 => "🇦🇬", // Antigua and Barbuda
            10 => "🇦🇷", // Argentina
            11 => "🇦🇲", // Aruba
            12 => "🇧🇸", // Bahamas
            13 => "🇧🇧", // Barbados
            14 => "🇧🇿", // Belize
            15 => "🇧🇴", // Bolivia
            16 => "🇧🇷", // Brazil
            17 => "🇻🇬", // British Virgin Islands
            18 => "🇨🇦", // Canada
            19 => "🇰🇾", // Cayman Islands
            20 => "🇨🇱", // Chile
            21 => "🇨🇴", // Colombia
            22 => "🇨🇷", // Costa Rica
            23 => "🇩🇲", // Dominica
            24 => "🇩🇴", // Dominican Republic
            25 => "🇪🇨", // Ecuador
            26 => "🇸🇻", // El Salvador
            27 => "🇫🇷", // French Guiana
            28 => "🇬🇩", // Grenada
            29 => "🇲🇶", // Guadeloupe
            30 => "🇵🇪", // Guatemala
            31 => "🇬🇾", // Guyana
            32 => "🇭🇹", // Haiti
            33 => "🇭🇳", // Honduras
            34 => "🇯🇲", // Jamaica
            35 => "🇲🇶", // Martinique
            36 => "🇲🇽", // Mexico
            37 => "🇲🇸", // Montserrat
            38 => "🇳🇱", // Netherlands Antilles
            39 => "🇳🇮", // Nicaragua
            40 => "🇵🇦", // Panama
            41 => "🇵🇾", // Paraguay
            42 => "🇵🇪", // Peru
            43 => "🇰🇳", // St. Kitts and Nevis
            44 => "🇱🇨", // St. Lucia
            45 => "🇻🇨", // St. Vincent and the Grenadines
            46 => "🇸🇷", // Suriname
            47 => "🇹🇹", // Trinidad and Tobago
            48 => "🇹🇨", // Turks and Caicos Islands
            49 => "🇺🇸", // United States
            50 => "🇺🇾", // Uruguay
            51 => "🇻🇮", // US Virgin Islands
            52 => "🇻🇪", // Venezuela

            // Europe & Africa
            64 => "🇦🇱", // Albania
            65 => "🇦🇺", // Australia
            66 => "🇦🇹", // Austria
            67 => "🇧🇪", // Belgium
            68 => "🇧🇦", // Bosnia and Herzegovina
            69 => "🇧🇼", // Botswana
            70 => "🇧🇬", // Bulgaria
            71 => "🇭🇷", // Croatia
            72 => "🇨🇾", // Cyprus
            73 => "🇨🇿", // Czech Republic
            74 => "🇩🇰", // Denmark
            75 => "🇪🇪", // Estonia
            76 => "🇫🇮", // Finland
            77 => "🇫🇷", // France
            78 => "🇩🇪", // Germany
            79 => "🇬🇷", // Greece
            80 => "🇭🇺", // Hungary
            81 => "🇮🇸", // Iceland
            82 => "🇮🇪", // Ireland
            83 => "🇮🇹", // Italy
            84 => "🇱🇻", // Latvia
            85 => "🇱🇸", // Lesotho
            86 => "🇱🇮", // Liechtenstein
            87 => "🇱🇹", // Lithuania
            88 => "🇱🇺", // Luxembourg
            89 => "🇲🇰", // North Macedonia
            90 => "🇲🇹", // Malta
            91 => "🇲🇪", // Montenegro
            92 => "🇲🇿", // Mozambique
            93 => "🇳🇦", // Namibia
            94 => "🇳🇱", // Netherlands
            95 => "🇳🇿", // New Zealand
            96 => "🇳🇴", // Norway
            97 => "🇵🇱", // Poland
            98 => "🇵🇹", // Portugal
            99 => "🇷🇴", // Romania
            100 => "🇷🇺", // Russia
            101 => "🇷🇸", // Serbia
            102 => "🇸🇰", // Slovakia
            103 => "🇸🇮", // Slovenia
            104 => "🇿🇦", // South Africa
            105 => "🇪🇸", // Spain
            106 => "🇸🇿", // Eswatini
            107 => "🇸🇪", // Sweden
            108 => "🇨🇭", // Switzerland
            109 => "🇹🇷", // Turkey
            110 => "🇬🇧", // United Kingdom
            111 => "🇿🇲", // Zambia
            112 => "🇿🇼", // Zimbabwe
            113 => "🇦🇿", // Azerbaijan
            114 => "🇲🇷", // Mauritania
            115 => "🇲🇱", // Mali
            116 => "🇳🇪", // Niger
            117 => "🇹🇩", // Chad
            118 => "🇸🇩", // Sudan
            119 => "🇪🇷", // Eritrea
            120 => "🇩🇯", // Djibouti
            121 => "🇸🇴", // Somalia

            // Southeast Asia
            128 => "🇹🇼", // Taiwan
            136 => "🇰🇷", // South Korea
            144 => "🇭🇰", // Hong Kong
            145 => "🇲🇴", // Macao
            152 => "🇮🇩", // Indonesia
            153 => "🇸🇬", // Singapore
            154 => "🇹🇭", // Thailand
            155 => "🇵🇭", // Philippines
            156 => "🇲🇾", // Malaysia
            160 => "🇨🇳", // China

            // Middle East
            168 => "🇦🇪", // U.A.E.
            169 => "🇮🇳", // India
            170 => "🇪🇬", // Egypt
            171 => "🇴🇲", // Oman
            172 => "🇶🇦", // Qatar
            173 => "🇰🇼", // Kuwait
            174 => "🇸🇦", // Saudi Arabia
            175 => "🇸🇾", // Syria
            176 => "🇧🇭", // Bahrain
            177 => "🇯🇴", // Jordan
            
            _ => "🏳️"
        };
    }
}
